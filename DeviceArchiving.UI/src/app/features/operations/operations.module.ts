import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OperationsRoutingModule } from './operations-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { OperationTypeFormComponent } from '../operation-types/components/operation-type-form/operation-type-form.component';
import { AddOperationDialogComponent } from './components/add-operation-dialog/add-operation-dialog.component';

@NgModule({
  declarations: [AddOperationDialogComponent],
  imports: [CommonModule,  SharedModule]
})
export class OperationsModule {}