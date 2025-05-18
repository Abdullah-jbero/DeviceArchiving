import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DeviceService } from '../../../../core/services/device.service';
import { Device } from '../../../../core/models/device.model';


@Component({
  selector: 'app-device-form',
  templateUrl: './device-form.component.html',
  styleUrls: ['./device-form.component.css'],
  standalone:false
})
export class DeviceFormComponent implements OnInit {
  form: FormGroup;
  isEditMode = false;
  deviceId?: number;

  constructor(
    private fb: FormBuilder,
    private deviceService: DeviceService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.form = this.fb.group({
      source: ['', Validators.required],
      brotherName: [''],
      laptopName: ['', Validators.required],
      systemPassword: ['',Validators.required],
      windowsPassword: ['',Validators.required],
      hardDrivePassword: ['',Validators.required],
      freezePassword: ['',Validators.required],
      code: ['',Validators.required],
      type: ['', Validators.required],
      serialNumber: ['', Validators.required],
      card: ['',Validators.required],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.deviceId = +this.route.snapshot.paramMap.get('id')!;
    if (this.deviceId) {
      this.isEditMode = true;
      this.loadDevice();
    }
  }

  loadDevice(): void {
    this.deviceService.getDeviceById(this.deviceId!).subscribe({
      next: (device) => this.form.patchValue(device),
      error: (err) => console.error('Error fetching device:', err)
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    const device: Device = {
      ...this.form.value,
      id: this.deviceId
    };

    if (this.isEditMode) {
      this.deviceService.updateDevice(device).subscribe({
        next: () => this.router.navigate(['/devices']),
        error: (err) => console.error('Error updating device:', err)
      });
    } else {
      this.deviceService.addDevice(device).subscribe({
        next: () => this.router.navigate(['/devices']),
        error: (err) => console.error('Error adding device:', err)
      });
    }
  }
}