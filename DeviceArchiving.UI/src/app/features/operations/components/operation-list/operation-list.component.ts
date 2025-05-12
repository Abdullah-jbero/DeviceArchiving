import { Component, OnInit } from '@angular/core';
import { Operation } from '../../../../core/models/operation.model';
import { OperationService } from '../../../../core/services/operation.service';

@Component({
  selector: 'app-operation-list',
  templateUrl: './operation-list.component.html',
  styleUrls: ['./operation-list.component.css'],
  standalone: false
})
export class OperationListComponent implements OnInit {
  operations: Operation[] = [];
  deviceId: number = 0; 

  constructor(private operationService: OperationService) {}

  ngOnInit(): void {
    this.loadOperations();
  }

  loadOperations(): void {
    if (this.deviceId) {
      this.operationService.getOperationsByDeviceId(this.deviceId).subscribe({
        next: (operations) => this.operations = operations,
        error: (err) => console.error('Error fetching operations:', err)
      });
    }
  }
}