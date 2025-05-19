import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';

const routes: Routes = [
  { path: 'devices', loadChildren: () => import('./features/devices/devices.module').then(m => m.DevicesModule), canActivate: [AuthGuard] },
  { path: 'operations', loadChildren: () => import('./features/operations/operations.module').then(m => m.OperationsModule), canActivate: [AuthGuard] },
  { path: 'operation-types', loadChildren: () => import('./features/operation-types/operation-types.module').then(m => m.OperationTypesModule), canActivate: [AuthGuard] },
  { path: 'account', loadChildren: () => import('./features/account/account.module').then(m => m.AccountModule) },
  { path: '', redirectTo: '/devices', pathMatch: 'full' },
  { path: '**', redirectTo: '/devices' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }