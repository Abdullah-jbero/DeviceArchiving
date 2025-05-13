import { Component, OnInit } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { OperationTypeService } from '../../../../core/services/operation-type.service';
import { OperationType } from '../../../../core/models/operation-type.model';
import { MessageService } from 'primeng/api';
import { Operation } from '../../../../core/models/operation.model';

interface DropdownOption {
  label: string;
  value: string;
}

@Component({
  selector: 'app-add-operation-dialog',
  templateUrl: `./add-operation-dialog.component.html`,
  styleUrls: [`./add-operation-dialog.component.css`,],
  standalone: false,
  providers: [MessageService]
})
export class AddOperationDialogComponent implements OnInit {
  deviceName: string;
  operation: Partial<Operation> = { operationName: '', oldValue: null, newValue: null };
  operationTypeOptions: DropdownOption[] = [];
  isLoading = false;

  constructor(
    private ref: DynamicDialogRef,
    private config: DynamicDialogConfig,
    private operationTypeService: OperationTypeService,
    private messageService: MessageService
  ) {
    this.deviceName = this.config.data.deviceName;
  }

  ngOnInit(): void {
    this.loadOperationTypes();
  }

  loadOperationTypes(): void {
    this.isLoading = true;
    this.operationTypeService.getOperationTypes().subscribe({
      next: (types: OperationType[]) => {
        this.operationTypeOptions = types.map((type) => ({
          label: type.name,
          value: type.name
        }));
        this.isLoading = false;
      },
      error: (err: any) => {
        this.isLoading = false;
        this.messageService.add({
          severity: 'error',
          summary: 'خطأ',
          detail: 'فشل تحميل أنواع العمليات.'
        });
        console.error('Error fetching operation types:', err);
      }
    });
  }

  save(): void {
    this.ref.close(this.operation);
  }

  cancel(): void {
    this.ref.close();
  }
}