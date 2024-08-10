import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-link-details',
  templateUrl: './link-details.component.html',
  styleUrls: ['./link-details.component.css']
})
export class LinkDetailsComponent implements OnInit {
  linkId!: string;
  username!: string;
  url!: string;
  expiryTime!: Date;
  maxClicks!: number;
  clickCount!: number;
  isExpired = false;
  message = '';
  information! : string;

  constructor(private route: ActivatedRoute, private http: HttpClient) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.linkId = params.get('id')!;
    });

    this.fetchLinkDetails();
  }

  fetchLinkDetails() {
    this.http.get(`https://localhost:7072/Admin/link-details/${this.linkId}`)
      .subscribe(
        (response: any) => {
         if (!response.isError){
            if (response.data.isExpired) {
              this.isExpired = true;
              this.information = response.information
            } else {
              this.isExpired = false;
              this.username = response.data.username;
              this.url = response.data.url;
              this.expiryTime = new Date(response.data.expiryTime);
              this.maxClicks = response.data.maxClicks;
              this.clickCount = response.data.clickCount;
              this.information = response.information
            }
         }else{
          this.information = response.information
         }
        },
        error => {
          console.error('Error!', error);
          this.message = 'An error occurred while fetching link details.';
        }
      );
  }
}
