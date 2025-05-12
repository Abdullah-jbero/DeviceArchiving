import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: 'devices', loadChildren: () => import('./features/devices/devices.module').then(m => m.DevicesModule) },
  { path: 'operations', loadChildren: () => import('./features/operations/operations.module').then(m => m.OperationsModule) },
  { path: 'operation-types', loadChildren: () => import('./features/operation-types/operation-types.module').then(m => m.OperationTypesModule) },
  { path: '', redirectTo: '/devices', pathMatch: 'full' },
  { path: '**', redirectTo: '/devices' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}