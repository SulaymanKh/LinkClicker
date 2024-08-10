import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { SignalRService } from '../services/signalr.service';
import { MatDialog } from '@angular/material/dialog';
import { LinkCreationDialogComponent } from '../link-creation-dialog/link-creation-dialog.component';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  form!: FormGroup;
  links: any[] = [];
  currentPage: number = 1;
  pageSize: number = 10;
  totalPages: number = 0;
  totalRecords: number = 0;
  isLoading: boolean = false;

  statusMap: { [key: number]: string } = {
    0: 'Active',
    1: 'Expired by Time',
    2: 'Expired by Clicks'
  };
  baseUrl = "https://localhost:7072/";

  constructor(
    private http: HttpClient,
    private router: Router,
    private signalRService: SignalRService,
    private dialog: MatDialog 
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

    this.signalRService.onLinkCreationCompleted((requestId: string, links: any[]) => {
      this.fetchAllLinks();
    });
  }

  onSubmit() {
    const linkData = {
      url: `http://localhost:4200/supersecret`,
      username: this.form.value.username,
      numberOfLinks: this.form.value.linkCount,
      clicksPerLink: this.form.value.allowedClicks,
      ExpiryInMinutes: this.form.value.expiryTime || null
    };

    if (this.form.valid) {
      this.isLoading = true;
      this.http.post(`${this.baseUrl}Admin/create-link`, linkData)
        .subscribe(
          () => {
            this.isLoading = false;
            this.dialog.open(LinkCreationDialogComponent, {
              width: '300px',
              data: { position: 'bottom-right' }, 
              panelClass: 'no-backdrop', 
              disableClose: true, 
              hasBackdrop: false 
            });
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

  fetchAllLinks(pageNumber: number = 1) {
    this.isLoading = true;

    const paginatedRequest = {
      pageNumber: pageNumber,
      pageSize: this.pageSize
    };

    this.http.post<any>(`${this.baseUrl}Admin/all-links`, paginatedRequest)
      .subscribe(
        response => {
          this.links = response.data;
          this.totalRecords = response.totalRecords;
          this.totalPages = response.totalPages;
          this.currentPage = response.pageNumber;
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

    this.http.delete(`${this.baseUrl}Admin/delete-links`, { body: requestBody })
      .subscribe(
        () => {
          this.fetchAllLinks(this.currentPage);
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

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.fetchAllLinks(page);
    }
  }

  navigateToLink(link: any) {
    window.open(link.link, '_blank');

    setTimeout(() => {
      this.fetchAllLinks();
    }, 1000); 
  }

  get startRecord(): number {
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  get endRecord(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalRecords);
  }
}
