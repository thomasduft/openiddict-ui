import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'api';

  public get apiUrl(): string {
    return `${this.baseUrl}`;
  }

  public createRawUrl(endpoint: string): string {
    return `${endpoint}`;
  }

  public createApiUrl(endpoint: string): string {
    return `${this.baseUrl}/${endpoint}`;
  }
}
