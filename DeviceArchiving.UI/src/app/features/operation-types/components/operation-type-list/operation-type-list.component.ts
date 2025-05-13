import { Component, OnInit } from '@angular/core';
import { OperationType } from '../../../../core/models/operation-type.model';
import { OperationTypeService } from '../../../../core/services/operation-type.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-operation-type-list',
  templateUrl: './operation-type-list.component.html',
  styleUrls: ['./operation-type-list.component.css'],
  standalone: false,
  providers: [MessageService ]
})
export class OperationTypeListComponent implements OnInit {
  operationTypes: OperationType[] = [];
  searchTerm: string = '';
  isLoading = false;
  private searchSubject = new Subject<string>();
  confirmedDeleteId: number | null = null;


  constructor(
    private operationTypeService: OperationTypeService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    // Debounce search input to avoid excessive API calls
    this.searchSubject.pipe(debounceTime(300)).subscribe((searchTerm) => {
      this.loadOperationTypes(searchTerm);
    });
    this.loadOperationTypes();
  }

  loadOperationTypes(searchTerm: string = this.searchTerm): void {
    this.isLoading = true;
    this.operationTypeService.getOperationTypes(searchTerm).subscribe({
      next: (types: OperationType[]) => {
        this.operationTypes = types;
        this.isLoading = false;
      },
      error: (err: any) => {
        this.isLoading = false;
        this.messageService.add({
          severity: 'error',
          summary: 'خطأ',
          detail: 'فشل تحميل أنواع العمليات.'
        });
        console.error('Error fetching operation types:', err);
      }
    });
  }

  onSearch(): void {
    this.searchSubject.next(this.searchTerm);
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.onSearch();
  }




  deleteOperationType(id: number): void {
    this.operationTypeService.deleteOperationType(id).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'نجاح',
          detail: 'تم حذف نوع العملية بنجاح.'
        });
        this.loadOperationTypes();
      },
      error: (err: any) => {
        this.messageService.add({
          severity: 'error',
          summary: 'خطأ',
          detail: 'فشل حذف نوع العملية.'
        });
        console.error('Error deleting operation type:', err);
      }
    });
  }
}