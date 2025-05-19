import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthenticationResponse } from '../models/authentication.model';
import { Router } from '@angular/router';

export interface AuthenticationRequest {
  email: string;
  password: string;
}

export interface BaseResponse<T> {
  success: boolean;
  data: T;
  message: string;
}

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private apiUrl = `${environment.apiBaseUrl}/api/Account`; // e.g., http://localhost:5000/api/account

  constructor(private http: HttpClient, private router: Router) {}

  authenticate(
    request: AuthenticationRequest
  ): Observable<BaseResponse<AuthenticationResponse>> {
    return this.http.post<BaseResponse<AuthenticationResponse>>(
      `${this.apiUrl}/authenticate`,
      request
    );
  }

  addUser(request: AuthenticationRequest): Observable<BaseResponse<string>> {
    return this.http.post<BaseResponse<string>>(
      `${this.apiUrl}/register`,
      request
    );
  }

  saveUserInfo(token: string, userName: string, base64Image: string): void {
    sessionStorage.setItem('authToken', token);
    sessionStorage.setItem('userName', userName);
    sessionStorage.setItem('userPicture', base64Image);
  }

  getUserInfo(): {
    token: string | null;
    userName: string | null;
    picture: string | null;
  } {
    const token = sessionStorage.getItem('authToken');
    const userName = sessionStorage.getItem('userName');
    const picture = sessionStorage.getItem('userPicture');

    return { token, userName, picture };
  }

  getToken(): string | null {
    return sessionStorage.getItem('authToken');
  }

  clearSession(): void {
    sessionStorage.clear();
  }
  isAuthenticated(): boolean {
    var isAuthenticated = !!this.getToken();
    if (isAuthenticated) {
      this.getUserInfo();
    }
    return isAuthenticated;
  }

  logout(): void {
    // Optionally, you can also redirect the user to the login page or show a message
    // this.router.navigate(['/login']);
    // this.toastr.success('Logged out successfully');
    this.clearSession();
    this.router.navigate(['/account/login']);
  }
}
