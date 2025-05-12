import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OperationsRoutingModule } from './operations-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { OperationTypeFormComponent } from '../operation-types/components/operation-type-form/operation-type-form.component';
import { OperationListComponent } from './components/operation-list/operation-list.component';

@NgModule({
  declarations: [OperationListComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, OperationsRoutingModule, SharedModule]
})
export class OperationsModule {}