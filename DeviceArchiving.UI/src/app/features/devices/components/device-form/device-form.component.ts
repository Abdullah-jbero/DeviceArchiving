import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DeviceService } from '../../../../core/services/device.service';
import { DevicesDto } from '../../../../core/models/device.model';
import { CreateDeviceDto } from '../../../../core/models/create-device.model';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-device-form',
  templateUrl: './device-form.component.html',
  styleUrls: ['./device-form.component.css'],
  standalone: false
})
export class DeviceFormComponent implements OnInit {
  form: FormGroup;
  isEditMode: boolean = false;
  deviceId: number | null = null;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private deviceService: DeviceService,
    private route: ActivatedRoute,
    private router: Router,
    private readonly messageService: MessageService,
  ) {
    this.form = this.fb.group({
      source: ['', Validators.required],
      brotherName: [''],
      laptopName: ['', Validators.required],
      systemPassword: ['', Validators.required],
      windowsPassword: ['', Validators.required],
      hardDrivePassword: ['', Validators.required],
      freezePassword: ['', Validators.required],
      code: ['', Validators.required],
      type: ['', Validators.required],
      serialNumber: ['', Validators.required],
      card: ['', Validators.required],
      comment: [''], // Added missing field
      contactNumber: [''], // Added missing field
      isActive: [true]
    });
  }

  ngOnInit(): void {
    // Check if we're in edit mode by looking for an ID in the route
    this.deviceId = Number(this.route.snapshot.paramMap.get('id'));
    this.isEditMode = !!this.deviceId;

    if (this.isEditMode && this.deviceId) {
      // Fetch device data for editing
      this.deviceService.getById(this.deviceId).subscribe({
        next: (device: DevicesDto) => {
          this.form.patchValue({
            source: device.source,
            brotherName: device.brotherName,
            laptopName: device.laptopName,
            systemPassword: device.systemPassword,
            windowsPassword: device.windowsPassword,
            hardDrivePassword: device.hardDrivePassword,
            freezePassword: device.freezePassword,
            code: device.code,
            type: device.type,
            serialNumber: device.serialNumber,
            card: device.card,
            comment: device.comment || '', // Handle null
            contactNumber: device.contactNumber || '', // Handle null
          });
        },

        error: (err) => {
          console.error('Error fetching device:', err);
          // Optionally show a toast notification
        }
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    const deviceData: CreateDeviceDto = {
      source: this.form.value.source,
      brotherName: this.form.value.brotherName,
      laptopName: this.form.value.laptopName,
      systemPassword: this.form.value.systemPassword,
      windowsPassword: this.form.value.windowsPassword,
      hardDrivePassword: this.form.value.hardDrivePassword,
      freezePassword: this.form.value.freezePassword,
      code: this.form.value.code,
      type: this.form.value.type,
      serialNumber: this.form.value.serialNumber,
      card: this.form.value.card,
      comment: this.form.value.comment || null,
      contactNumber: this.form.value.contactNumber || null

    };

    if (this.isEditMode && this.deviceId) {
      // Update existing device
      this.deviceService.update(this.deviceId, deviceData).subscribe({
        next: () => {
          this.router.navigate(['/devices']);
        },
        error: (err) => {
          this.messageService.add({
            severity: 'error',
            summary: 'خطأ',
            detail: err.message || 'حدث خطأ في تحديث الجهاز',
          });
          this.loading = false;
          // Optionally show a toast notification
        }
      });
    } else {
      // Create new device
      this.deviceService.create(deviceData).subscribe({
        next: () => {
          this.router.navigate(['/devices']);
        },
        error: (err) => {
          this.messageService.add({
            severity: 'error',
            summary: 'خطأ',
            detail: err.message || 'حدث خطأ في انشاء الجهاز',
          });
          this.loading = false;
          // Optionally show a toast notification

        }
      });
    }
  }
}