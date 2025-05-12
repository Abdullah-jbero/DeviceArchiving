import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Operation } from '../models/operation.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OperationService {
  private apiUrl = `${environment.apiBaseUrl}/api/Operations`;

  constructor(private http: HttpClient) {}

  getOperationsByDeviceId(deviceId: number): Observable<Operation[]> {
    return this.http.get<Operation[]>(`${this.apiUrl}/drive/${deviceId}`);
  }

  addOperation(operation: Operation): Observable<Operation> {
    return this.http.post<Operation>(this.apiUrl, operation);
  }
}