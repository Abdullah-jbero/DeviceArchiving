import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DevicesDto, DeviceDto } from '../models/device.model';
import { CreateDeviceDto } from '../models/create-device.model';
import { UpdateDeviceDto } from '../models/update-device.model';


@Injectable({
  providedIn: 'root'
})
export class DeviceService {
  private apiUrl = `${environment.apiBaseUrl}/api/Devices`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<DevicesDto[]> {
    return this.http.get<DevicesDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<DeviceDto> {
    return this.http.get<DeviceDto>(`${this.apiUrl}/${id}`);
  }

  create(device: CreateDeviceDto): Observable<void> {
    return this.http.post<void>(this.apiUrl, device);
  }

  update(id: number, device: UpdateDeviceDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, device);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
