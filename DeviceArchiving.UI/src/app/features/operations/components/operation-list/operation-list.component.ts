import { Component } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { OperationDto } from '../../../../core/models/device.model';

@Component({
  selector: 'app-operation-list',
  standalone: false,
  templateUrl: './operation-list.component.html',
  styleUrl: './operation-list.component.css'
})
export class OperationListComponent {
  operations: OperationDto[] = [];

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) { }

  ngOnInit(): void {
    // Retrieve operations from dialog data
    this.operations = this.config.data?.operations || [];
    console.log('Operations loaded:', this.operations);
  }

  closeDialog(): void {
    this.ref.close();
  }
}
