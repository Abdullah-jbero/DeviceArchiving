import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LoginComponent } from './components/login/login.component';
import { SignupComponent } from './components/signup/signup.component';
import { AccountRoutingModule } from './account-routing.module';
import { SharedModule } from '../../shared/shared.module';



@NgModule({
  declarations: [
    LoginComponent,
    SignupComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    AccountRoutingModule,
    FormsModule,
    
  ]
})
export class AccountModule { }