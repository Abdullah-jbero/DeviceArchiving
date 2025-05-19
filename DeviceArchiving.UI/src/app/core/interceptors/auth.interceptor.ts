import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private accountService: AccountService,
    private messageService: MessageService
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.accountService.getToken();

    // Clone request and add Authorization header if token exists
    let authReq = req;
    if (token) {
      authReq = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    } else {
      console.warn('No token found, request will not be authenticated.');
      // Optionally, skip logout here if the request is for a public endpoint (e.g., login)
      // For now, proceed without token
    }

    return next.handle(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          this.messageService.add({
            severity: 'error',
            summary: 'انتهت الجلسة',
            detail: 'تم تسجيل خروجك بسبب انتهاء الجلسة. الرجاء تسجيل الدخول مرة أخرى.'
          });
          this.accountService.logout();
        
        }
        return throwError(() => error);
      })
    );
  }
}