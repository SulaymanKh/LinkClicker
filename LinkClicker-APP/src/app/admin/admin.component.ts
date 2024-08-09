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

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router 
  ) {}

  ngOnInit() {
    this.form = new FormGroup({
      username: new FormControl('', [Validators.required]),
      linkCount: new FormControl('', [Validators.required, Validators.min(1)]),
      allowedClicks: new FormControl('', [Validators.required, Validators.min(1)]),
      expiryTime: new FormControl('', [Validators.required, Validators.min(1)])
    });
  }

  onSubmit() {
    const linkData = {
      url: `${this.baseUrl}supersecret`, 
      username: this.form.value.username,
      numberOfLinks: this.form.value.linkCount,
      clicksPerLink: this.form.value.allowedClicks,
      ExpiryInMinutes: this.form.value.expiryTime
    };

    if (this.form.valid) {
      const fullUrl = `https://localhost:7072/Admin/create-link`;
      this.http.post(fullUrl, linkData)
        .subscribe(
          (response: any) => {
            this.links = response.data;
            this.updatePagination(); 
          },
          error => console.error('Error!', error)
        );
    } else {
      console.log('Form is not valid');
    }
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
    window.open(link.link, '_blank');
  }
}
