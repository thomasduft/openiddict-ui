import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  public setSessionItem<T>(key: string, item: T): void {
    sessionStorage.setItem(key, JSON.stringify(item));
  }

  public getSessionItem<T>(key: string): T {
    return JSON.parse(sessionStorage.getItem(key)) as T;
  }

  public removeSessionItem(key: string): void {
    sessionStorage.removeItem(key);
  }

  public setItem<T>(key: string, item: T): void {
    localStorage.setItem(key, JSON.stringify(item));
  }

  public getItem<T>(key: string): T {
    return JSON.parse(localStorage.getItem(key)) as T;
  }

  public removeItem(key: string): void {
    localStorage.removeItem(key);
  }
}
