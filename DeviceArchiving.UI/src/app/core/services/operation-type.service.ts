import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OperationType } from '../models/operation-type.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OperationTypeService {
  private apiUrl = `${environment.apiBaseUrl}/api/OperationTypes`;

  constructor(private http: HttpClient) {}

  getOperationTypes(searchTerm?: string): Observable<OperationType[]> {
    const url = searchTerm ? `${this.apiUrl}?searchTerm=${encodeURIComponent(searchTerm)}` : this.apiUrl;
    return this.http.get<OperationType[]>(url);
  }

  getOperationTypeById(id: number): Observable<OperationType> {
    return this.http.get<OperationType>(`${this.apiUrl}/${id}`);
  }

  addOperationType(operationType: OperationType): Observable<OperationType> {
    return this.http.post<OperationType>(this.apiUrl, operationType);
  }

  updateOperationType(operationType: OperationType): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${operationType.id}`, operationType);
  }

  deleteOperationType(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}