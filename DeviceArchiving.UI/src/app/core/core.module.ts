import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { DeviceService } from './services/device.service';
import { OperationService } from './services/operation.service';
import { OperationTypeService } from './services/operation-type.service';

@NgModule({
  imports: [CommonModule, HttpClientModule],
  providers: [DeviceService, OperationService, OperationTypeService]
})
export class CoreModule {}