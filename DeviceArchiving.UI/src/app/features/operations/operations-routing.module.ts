import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OperationTypeFormComponent } from '../operation-types/components/operation-type-form/operation-type-form.component';


const routes: Routes = [
  { path: 'add', component: OperationTypeFormComponent },
  { path: 'edit/:id', component: OperationTypeFormComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OperationsRoutingModule {}