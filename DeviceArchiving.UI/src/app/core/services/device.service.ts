import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { DevicesDto, DeviceDto } from '../models/device.model';
import { CreateDeviceDto } from '../models/create-device.model';
import { BaseResponse, UpdateDeviceDto } from '../models/update-device.model';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {
  private readonly apiUrl: string = `${environment.apiBaseUrl}/api/Devices`;

  constructor(private readonly http: HttpClient) { }

  uploadFile(formData: FormData): Observable<BaseResponse<number>> {
    return this.http.post<BaseResponse<number>>(`${this.apiUrl}/upload`, formData).pipe(
      catchError(this.handleError)
    );
  }

  getAll(): Observable<DevicesDto[]> {
    return this.http.get<DevicesDto[]>(this.apiUrl)
      .pipe(catchError(this.handleError));
  }

  getById(id: number): Observable<DeviceDto> {
    return this.http.get<DeviceDto>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  create(device: CreateDeviceDto): Observable<void> {
    return this.http.post<void>(this.apiUrl, device)
      .pipe(catchError(this.handleError));
  }

  update(id: number, device: UpdateDeviceDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, device)
      .pipe(catchError(this.handleError));
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }


  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('An error occurred:', error);
    let errorMessage = 'حدث خطأ غير متوقع';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `خطأ في العميل: ${error.error.message}`;
    } else {
      // Server-side error
      if (error.error?.message) {
        errorMessage = error.error.message;
      }
    }

    return throwError(() => new Error(errorMessage));
  }


}