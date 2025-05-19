import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OperationType } from '../../../../core/models/operation-type.model';
import { Operation } from '../../../../core/models/operation.model';
import { DeviceService } from '../../../../core/services/device.service';
import { OperationTypeService } from '../../../../core/services/operation-type.service';
import { OperationService } from '../../../../core/services/operation.service';
import { DeviceDto } from '../../../../core/models/device.model';


@Component({
  selector: 'app-device-details',
  templateUrl: './device-details.component.html',
  styleUrls: ['./device-details.component.css'],
  standalone: false,
})
export class DeviceDetailsComponent implements OnInit {
  device: DeviceDto | null = null;
  operations: Operation[] = [];
  operationTypes: OperationType[] = [];
  operationForm: FormGroup;
  deviceId: number;

  constructor(
    private route: ActivatedRoute,
    private deviceService: DeviceService,
    private operationService: OperationService,
    private operationTypeService: OperationTypeService,
    private fb: FormBuilder
  ) {
    this.deviceId = +this.route.snapshot.paramMap.get('id')!;
    this.operationForm = this.fb.group({
      operationTypeId: ['', Validators.required],
      details: ['']
    });
  }

  ngOnInit(): void {
    this.loadDevice();
    this.loadOperations();
    this.loadOperationTypes();
  }

  loadDevice(): void {
    this.deviceService.getById(this.deviceId).subscribe({
      next: (device) => this.device = device,
      error: (err) => console.error('Error fetching device:', err)
    });
  }

  loadOperations(): void {
    this.operationService.getOperationsByDeviceId(this.deviceId).subscribe({
      next: (operations) => this.operations = operations,
      error: (err) => console.error('Error fetching operations:', err)
    });
  }

  loadOperationTypes(): void {
    this.operationTypeService.getOperationTypes().subscribe({
      next: (types) => this.operationTypes = types,
      error: (err) => console.error('Error fetching operation types:', err)
    });
  }

  addOperation(): void {
    if (this.operationForm.invalid) return;

    const operation: Operation = {
      deviceId: this.deviceId,
      operationName: this.operationForm.value.details,
      createdAt: new Date().toLocaleDateString('en-US')
    };

    this.operationService.addOperation(operation).subscribe({
      next: () => {
        this.loadOperations();
        this.operationForm.reset();
      },
      error: (err) => console.error('Error adding operation:', err)
    });
  }
}