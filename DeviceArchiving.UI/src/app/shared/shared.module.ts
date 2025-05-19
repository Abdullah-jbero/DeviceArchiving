import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './components/header/header/header.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// PrimeNG
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ToastModule } from 'primeng/toast';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

// Angular Material
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule }  from '@angular/material/button';
import { MatIconModule }    from '@angular/material/icon';
import { MatMenuModule }    from '@angular/material/menu';
import { MatListModule }    from '@angular/material/list';
import { MatRippleModule }  from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
@NgModule({
  declarations: [
    HeaderComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,

    // PrimeNG
    TableModule,
    ButtonModule,
    InputTextModule,
    DropdownModule,
    ToastModule,
    ProgressSpinnerModule,

    // Angular Material
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatListModule,
    MatRippleModule,
    MatFormFieldModule,
    MatCardModule,
    MatSnackBarModule
  ],
  exports: [
    // Components
    HeaderComponent,

    // Angular core
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,

    // PrimeNG
    TableModule,
    ButtonModule,
    InputTextModule,
    DropdownModule,
    ToastModule,
    ProgressSpinnerModule,
    DynamicDialogModule,

    // Angular Material
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatListModule,
    MatRippleModule,
    MatFormFieldModule,
    MatCardModule,
    MatSnackBarModule
  ]
})
export class SharedModule { }
