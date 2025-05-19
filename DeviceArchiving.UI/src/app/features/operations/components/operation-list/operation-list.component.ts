import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { OperationDto } from '../../../../core/models/device.model';
import { JsonPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-operation-list',
  templateUrl: './operation-list.component.html',
  styleUrls: ['./operation-list.component.css'],
  standalone: true,
  imports: [CommonModule , TableModule, ButtonModule, ToastModule]

}) 
export class OperationListComponent implements OnInit {
  operations: OperationDto[] = [];

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.operations = this.config.data?.operations || [];
    console.log('Operations loaded:', this.operations);
    this.cdr.detectChanges();
  }

  closeDialog(): void {
    this.ref.close();
  }
}