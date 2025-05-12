import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { OperationTypeService } from '../../../../core/services/operation-type.service';
import { OperationType } from '../../../../core/models/operation-type.model';

@Component({
  selector: 'app-operation-type-form',
  templateUrl: './operation-type-form.component.html',
  styleUrls: ['./operation-type-form.component.css'],
  standalone :false
})
export class OperationTypeFormComponent implements OnInit {
  form: FormGroup;
  isEditMode = false;
  operationTypeId?: number;

  constructor(
    private fb: FormBuilder,
    private operationTypeService: OperationTypeService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit(): void {
    this.operationTypeId = +this.route.snapshot.paramMap.get('id')!;
    if (this.operationTypeId) {
      this.isEditMode = true;
      this.loadOperationType();
    }
  }

  loadOperationType(): void {
    this.operationTypeService.getOperationTypeById(this.operationTypeId!).subscribe({
      next: (type) => this.form.patchValue(type),
      error: (err) => console.error('Error fetching operation type:', err)
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    const operationType: OperationType = {
      ...this.form.value,
      id: this.operationTypeId
    };

    if (this.isEditMode) {
      this.operationTypeService.updateOperationType(operationType).subscribe({
        next: () => this.router.navigate(['/operation-types']),
        error: (err) => console.error('Error updating operation type:', err)
      });
    } else {
      this.operationTypeService.addOperationType(operationType).subscribe({
        next: () => this.router.navigate(['/operation-types']),
        error: (err) => console.error('Error adding operation type:', err)
      });
    }
  }
}