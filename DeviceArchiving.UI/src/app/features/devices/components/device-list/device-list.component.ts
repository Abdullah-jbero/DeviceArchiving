import { Component, OnInit } from '@angular/core';
import { Device } from '../../../../core/models/device.model';
import { Operation } from '../../../../core/models/operation.model';
import { DeviceService } from '../../../../core/services/device.service';
import { OperationService } from '../../../../core/services/operation.service';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.css'],
  standalone: false
})
export class DeviceListComponent implements OnInit {
  devices: Device[] | any[] = [];
  filteredDevices: Device[] = [];
  selectedDevice: Device | null = null;
  operations: Operation[] = [];
  globalSearchQuery: string = '';
  searchCriteria = {
    laptopName: '',
    serialNumber: '',
    type: ''
  };
  deviceTypes: { label: string; value: string }[] = [];
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

  constructor(
    private deviceService: DeviceService,
    private operationService: OperationService
  ) { }

  ngOnInit(): void {
    this.deviceService.getAllDevices().subscribe(devices => {
      this.devices = devices.map(device => ({
        ...device,
        createdAt: new Date(device.createdAt)
      }));
      this.filteredDevices = [...this.devices];
      this.deviceTypes = [...new Set(this.devices.map(device => device.type))].map(type => ({
        label: type,
        value: type
      }));
      console.log(this.deviceTypes);
    });
  }

  selectDevice(device: Device): void {
    this.selectedDevice = device;
    this.operationService.getOperationsByDeviceId(device.id!).subscribe((operations) => {
      this.operations = operations;
    });
  }

  applyFilter(): void {
    let filtered = this.devices;

    if (this.globalSearchQuery.trim()) {
      const query = this.globalSearchQuery.toLowerCase();
      filtered = filtered.filter(device =>
        (device.laptopName?.toLowerCase().includes(query) || false) ||
        (device.serialNumber?.toLowerCase().includes(query) || false) ||
        (device.source?.toLowerCase().includes(query) || false) ||
        (device.brotherName?.toLowerCase().includes(query) || false) ||
        (device.type?.toLowerCase().includes(query) || false) ||
        (device.code?.toLowerCase().includes(query) || false)
      );
    }

    filtered = filtered.filter((device) => {
      return (
        (!this.searchCriteria.laptopName ||
          device.laptopName.toLowerCase().includes(this.searchCriteria.laptopName.toLowerCase())) &&
        (!this.searchCriteria.serialNumber ||
          device.serialNumber.toLowerCase().includes(this.searchCriteria.serialNumber.toLowerCase())) &&
        (!this.searchCriteria.type || device.type === this.searchCriteria.type)
      );
    });

    this.filteredDevices = filtered;
  }

  clearSearch(): void {
    this.globalSearchQuery = '';
    this.searchCriteria = { laptopName: '', serialNumber: '', type: '' };
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

  exportToExcel(): void {
    // Map filteredDevices to the desired Excel format
    const data = this.filteredDevices.map(device => ({
      'اسم اللاب توب': device.laptopName,
      'الرقم التسلسلي': device.serialNumber,
      'النوع': device.type,
      'الجهة': device.source,
      'اسم الأخ': device.brotherName,
      'كلمة مرور النظام': device.systemPassword,
      'كلمة مرور ويندوز': device.windowsPassword,
      'كلمة التشفير': device.hardDrivePassword,
      'كلمة التجميد': device.freezePassword,
      'الكود': device.code,
      'الكرت': device.card,
      'تاريخ الإنشاء': device.createdAt.toString(),
      'الحالة': device.isActive ? 'نشط' : 'غير نشط'
    }));

    // Create worksheet from data
    const worksheet = XLSX.utils.json_to_sheet(data);

    // Create workbook and append worksheet
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'الأجهزة');

    // Generate Excel file
    const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const blob = new Blob([excelBuffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });

    // Create a temporary URL and trigger download
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'devices.xlsx';
    document.body.appendChild(link);
    link.click();

    // Clean up
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }
}