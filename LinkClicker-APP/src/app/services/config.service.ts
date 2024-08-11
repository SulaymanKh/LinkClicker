import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private readonly _webAppUrl: string = 'http://localhost:4200/';
  private readonly _webApiUrl: string = 'https://localhost:7072/';

  get webAppUrl(): string {
    return this._webAppUrl;
  }

  get webApiUrl(): string {
    return this._webApiUrl;
  }
}
