import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { OperationTypeService } from '../../../../core/services/operation-type.service';
import { OperationType } from '../../../../core/models/operation-type.model';
import { MessageService } from 'primeng/api';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-operation-type-form',
  templateUrl: './operation-type-form.component.html',
  styleUrls: ['./operation-type-form.component.css'],
  standalone: false,
  providers: [MessageService]
})
export class OperationTypeFormComponent implements OnInit {
  form: FormGroup;
  isEditMode = false;
  operationTypeId?: number;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private operationTypeService: OperationTypeService,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(500)]],
    });
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const parsedId = parseInt(idParam, 10);
      if (!isNaN(parsedId)) {
        this.operationTypeId = parsedId;
        this.isEditMode = true;
        this.loadOperationType();
      } else {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'معرف نوع العملية غير صالح.' });
        this.router.navigate(['/operation-types']);
      }
    }
  }

  loadOperationType(): void {
    if (!this.operationTypeId) return;

    this.isLoading = true;
    this.operationTypeService.getOperationTypeById(this.operationTypeId).subscribe({
      next: (type: OperationType) => {
        this.form.patchValue(type);
        this.isLoading = false;
      },
      error: (err: any) => {
        this.isLoading = false;
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل نوع العملية.' });
        console.error('Error fetching operation type:', err);
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const operationType: OperationType = {
      ...this.form.value,
      id: this.isEditMode ? this.operationTypeId : undefined
    };

    let operation: Observable<OperationType | void> = this.isEditMode
      ? this.operationTypeService.updateOperationType(operationType)
      : this.operationTypeService.addOperationType(operationType);

    operation.subscribe({
      next: (result) => {
        this.isLoading = false;
        this.messageService.add({
          severity: 'success',
          summary: 'نجاح',
          detail: this.isEditMode ? 'تم تحديث نوع العملية بنجاح.' : 'تم إضافة نوع العملية بنجاح.'
        });
        this.router.navigate(['/operation-types']);
      },
      error: (err) => {
        this.isLoading = false;
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء حفظ نوع العملية.' });
        console.error(`Error ${this.isEditMode ? 'updating' : 'adding'} operation type:`, err);
      }
    });



  }

  cancel(): void {
    this.router.navigate(['/operation-types']);
  }
}