import { Component, OnInit } from '@angular/core';

import { Device } from '../../../../core/models/device.model';
import { Operation } from '../../../../core/models/operation.model';
import { DeviceService } from '../../../../core/services/device.service';
import { OperationService } from '../../../../core/services/operation.service';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.css'],
 standalone:false
})
export class DeviceListComponent implements OnInit {
  devices: Device[] | any[] = [];
  filteredDevices: Device[] = [];
  selectedDevice: Device | null = null;
  operations: Operation[] = [];
  searchCriteria = {
    laptopName: '',
    serialNumber: '',
    type: ''
  };
  deviceTypes = [];
  filters: any = {
    global: { value: null, matchMode: 'contains' },
    source: { value: null, matchMode: 'contains' },
    brotherName: { value: null, matchMode: 'contains' },
    laptopName: { value: null, matchMode: 'contains' },
    type: { value: null, matchMode: 'equals' },
    serialNumber: { value: null, matchMode: 'contains' },
    isActive: { value: null, matchMode: 'equals' },
    createdAt: { value: null, matchMode: 'dateIs' }
  };
  globalFilter: string | null = null;

  constructor(
    private deviceService: DeviceService,
    private operationService: OperationService
  ) {}

  ngOnInit(): void {
    this.deviceService.getAllDevices().subscribe((devices) => {
      this.devices = devices.map(device => ({
        ...device,
        createdAt: new Date(device.createdAt)
      }));
      this.filteredDevices = this.devices;
    });
  }

  selectDevice(device: Device): void {
    this.selectedDevice = device;
    this.operationService.getOperationsByDeviceId(device.id!).subscribe((operations) => {
      this.operations = operations;
    });
  }

  applyFilter(): void {
    this.filteredDevices = this.devices.filter((device) => {
      return (
        (!this.searchCriteria.laptopName ||
          device.laptopName.toLowerCase().includes(this.searchCriteria.laptopName.toLowerCase())) &&
        (!this.searchCriteria.serialNumber ||
          device.serialNumber.toLowerCase().includes(this.searchCriteria.serialNumber.toLowerCase())) &&
        (!this.searchCriteria.type || device.type === this.searchCriteria.type)
      );
    });
  }

  clearSearch(): void {
    this.searchCriteria = { laptopName: '', serialNumber: '', type: '' };
    this.globalFilter = null;
    this.filters = {
      global: { value: null, matchMode: 'contains' },
      source: { value: null, matchMode: 'contains' },
      brotherName: { value: null, matchMode: 'contains' },
      laptopName: { value: null, matchMode: 'contains' },
      type: { value: null, matchMode: 'equals' },
      serialNumber: { value: null, matchMode: 'contains' },
      isActive: { value: null, matchMode: 'equals' },
      createdAt: { value: null, matchMode: 'dateIs' }
    };
    this.filteredDevices = this.devices;
  }
}