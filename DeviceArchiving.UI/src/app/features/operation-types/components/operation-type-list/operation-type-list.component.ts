import { Component, OnInit } from '@angular/core';
import { OperationType } from '../../../../core/models/operation-type.model';
import { OperationTypeService } from '../../../../core/services/operation-type.service';


@Component({
  selector: 'app-operation-type-list',
  templateUrl: './operation-type-list.component.html',
  styleUrls: ['./operation-type-list.component.css'],
  standalone:false
})
export class OperationTypeListComponent implements OnInit {
  operationTypes: OperationType[] = [];
  searchTerm: string = '';

  constructor(private operationTypeService: OperationTypeService) {}

  ngOnInit(): void {
    this.loadOperationTypes();
  }

  loadOperationTypes(): void {
    this.operationTypeService.getOperationTypes(this.searchTerm).subscribe({
      next: (types) => this.operationTypes = types,
      error: (err) => console.error('Error fetching operation types:', err)
    });
  }

  deleteOperationType(id: number): void {
    if (confirm('Are you sure you want to delete this operation type?')) {
      this.operationTypeService.deleteOperationType(id).subscribe({
        next: () => this.loadOperationTypes(),
        error: (err) => console.error('Error deleting operation type:', err)
      });
    }
  }
}