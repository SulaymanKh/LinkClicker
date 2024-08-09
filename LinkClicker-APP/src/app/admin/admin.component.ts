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

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router 
  ) {}

  ngOnInit() {
    this.form = new FormGroup({
      username: new FormControl('', [Validators.required]),
      linkCount: new FormControl('', [Validators.required, Validators.min(1)]),
      expiryTime: new FormControl('', [Validators.required, Validators.min(1)])
    });
  }

  onSubmit() {
    const linkData = {
      url: 'http://localhost:4200/supersecret', 
      username: this.form.value.username,
      numberOfLinks: this.form.value.linkCount,
      clicksPerLink: 1,
      expiryInHours: this.form.value.expiryTime
    };

    if (this.form.valid) {
      const fullUrl = `https://localhost:7072/Admin/create-link`;
      this.http.post(fullUrl, linkData)
        .subscribe(
          (response: any) => {
            console.log('Success!', response);
            this.links = response.data;
          },
          error => console.error('Error!', error)
        );
    } else {
      console.log('Form is not valid');
    }
  }

  navigateToLink(link: any) {
    window.open(link.link, '_blank');
  }
}
