import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OperationTypesRoutingModule } from './operation-types-routing.module';
import { OperationTypeFormComponent } from './components/operation-type-form/operation-type-form.component';
import { SharedModule } from '../../shared/shared.module';
import { OperationTypeListComponent } from './components/operation-type-list/operation-type-list.component';
import { ToastModule } from 'primeng/toast';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
@NgModule({
  declarations: [OperationTypeListComponent, OperationTypeFormComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    OperationTypesRoutingModule,
    SharedModule
  ]
})
export class OperationTypesModule { }