import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  form!: FormGroup;
  links: any[] = [];
  paginatedLinks: any[] = []; 
  currentPage: number = 1; 
  pageSize: number = 10;
  totalPages: number = 0;
  isLoading: boolean = false;

  statusMap: { [key: number]: string } = {
    0: 'Active',
    1: 'Expired by Time',
    2: 'Expired by Clicks'
  };

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router 
  ) {}

  ngOnInit() {
    this.fetchAllLinks();

    this.form = new FormGroup({
      username: new FormControl('', [
        Validators.required,
        Validators.maxLength(50),
        Validators.pattern('^[a-zA-Z0-9_]*$')
      ]),
      linkCount: new FormControl('', [Validators.required, Validators.min(1)]),
      allowedClicks: new FormControl('', [Validators.required, Validators.min(1)]),
      expiryTime: new FormControl(null)
    });
  }

  onSubmit() {
    const linkData = {
      url: `${this.baseUrl}supersecret`, 
      username: this.form.value.username,
      numberOfLinks: this.form.value.linkCount,
      clicksPerLink: this.form.value.allowedClicks,
      ExpiryInMinutes: this.form.value.expiryTime || null
    };

    if (this.form.valid) {
      this.isLoading = true;  
      const fullUrl = `https://localhost:7072/Admin/create-link`;
      this.http.post(fullUrl, linkData)
        .subscribe(
          (response: any) => {
            this.fetchAllLinks();
            this.isLoading = false;  
          },
          error => {
            console.error('Error!', error);
            this.isLoading = false;  
          }
        );
    } else {
      console.log('Form is not valid');
    }
  }

  fetchAllLinks(){
    this.isLoading = true; 

    const fullUrl = `https://localhost:7072/Admin/all-links`;
    this.http.get(fullUrl)
      .subscribe(
        (response: any) => {
          this.links = response.data;
          this.updatePagination(); 
          this.isLoading = false;  
        },
        error => {
          console.error('Error!', error);
          this.isLoading = false; 
        }
      );
  }

  deleteLinks(deleteAll: boolean, statuses: number[]) {
    this.isLoading = true; 
    
    const requestBody = {
      deleteAll: deleteAll,
      statuses: statuses
    };

    this.http.delete(`https://localhost:7072/Admin/delete-links`, { body: requestBody })
      .subscribe(
        (response: any) => {
          this.fetchAllLinks();
          this.isLoading = false;  
        },
        error => {
          console.error('Error!', error);
          this.isLoading = false;  
        }
      );
  }

  deleteAllLinks() {
    this.deleteLinks(true, []);
  }

  deleteExpiredByTime() {
    this.deleteLinks(false, [1]); 
  }

  deleteExpiredByClicks() {
    this.deleteLinks(false, [2]); 
  }

  getStatusText(statusCode: number): string {
    return this.statusMap[statusCode] || 'Unknown';
  }

  updatePagination() {
    this.totalPages = Math.ceil(this.links.length / this.pageSize);
    this.paginate();
  }

  paginate() {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedLinks = this.links.slice(startIndex, endIndex);
  }

  changePage(page: number) {
    this.currentPage = page;
    this.paginate();
  }

  navigateToLink(link: any) {
    this.fetchAllLinks();
    window.open(link.link, '_blank');
  }
}
