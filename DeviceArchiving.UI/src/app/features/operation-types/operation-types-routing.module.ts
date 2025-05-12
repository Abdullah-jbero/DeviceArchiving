import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OperationTypeListComponent } from './components/operation-type-list/operation-type-list.component';
import { OperationTypeFormComponent } from './components/operation-type-form/operation-type-form.component';

const routes: Routes = [
  { path: '', component: OperationTypeListComponent },
  { path: 'add', component: OperationTypeFormComponent },
  { path: 'edit/:id', component: OperationTypeFormComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OperationTypesRoutingModule {}