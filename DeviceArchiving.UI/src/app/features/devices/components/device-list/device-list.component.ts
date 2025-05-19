import { Component, OnInit } from '@angular/core';

import { CreateOperation, Operation } from '../../../../core/models/operation.model';
import { DeviceService } from '../../../../core/services/device.service';
import { OperationService } from '../../../../core/services/operation.service';
import * as XLSX from 'xlsx';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { AddOperationDialogComponent } from '../../../operations/components/add-operation-dialog/add-operation-dialog.component';
import { DevicesDto, OperationDto } from '../../../../core/models/device.model';
import { OperationListComponent } from '../../../operations/components/operation-list/operation-list.component';


@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.css'],
  standalone: false,
  providers: [DialogService, MessageService]
})
export class DeviceListComponent implements OnInit {
  devices: DevicesDto[] | any[] = [];
  filteredDevices: DevicesDto[] = [];
  selectedDevice: DevicesDto | null = null;
  operations: OperationDto[] = [];
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
  darkMode: boolean = false;
  dialogRef: DynamicDialogRef | null = null;
  loading: boolean = false;

  constructor(
    private deviceService: DeviceService,
    private operationService: OperationService,
    private dialogService: DialogService,
    private messageService: MessageService
  ) { }



  ngOnInit(): void {
    this.loading = true;
    this.deviceService.getAll().subscribe({
      next: (devices) => {
        this.devices = devices;
        this.filteredDevices = [...this.devices];
        this.deviceTypes = [...new Set(this.devices.map(device => device.type))].map(type => ({
          label: type,
          value: type
        }));
        this.loading = false;
      },

      error: () => {
        this.loading = false;
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل الأجهزة.' });
      }
    });
  }

  selectDevice(device: DevicesDto): void {
    this.selectedDevice = device;
  }

  showOperations(device: DevicesDto): void {
    this.deviceService.getById(device.id!).subscribe({
      next: (response) => {
        this.operations = response.operationsDtos.map(op => ({
          operationName: op.operationName || op.operationName,
          oldValue: op.oldValue || op.oldValue,
          newValue: op.newValue || op.newValue,
          comment: op.comment,
          userName: op.userName || op.userName,
          createdAt: op.createdAt || op.createdAt
        }));
        console.log('Mapped operations:', this.operations); // Debug
        this.dialogRef = this.dialogService.open(OperationListComponent, {
          header: 'العمليات',
          width: '70%',
          contentStyle: { direction: 'rtl', padding: '1rem' },
          data: { operations: this.operations }
        });
      },
      error: (error) => {
        console.error('Error fetching operations:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'خطأ',
          detail: 'فشل جلب العمليات. حاول مرة أخرى.'
        });
      }
    });
  }
  applyFilter(): void {
    let filtered = this.devices;

    if (this.globalSearchQuery.trim()) {
      const query = this.globalSearchQuery.toLowerCase();
      filtered = filtered.filter(device =>
        ['source', 'brotherName', 'laptopName', 'systemPassword', 'windowsPassword',
          'hardDrivePassword', 'freezePassword', 'code', 'card', 'serialNumber', 'type']
          .some(key => device[key]?.toLowerCase().includes(query))
      );
    }

    filtered = filtered.filter(device =>
      (!this.searchCriteria.laptopName ||
        device.laptopName.toLowerCase().includes(this.searchCriteria.laptopName.toLowerCase())) &&
      (!this.searchCriteria.serialNumber ||
        device.serialNumber.toLowerCase().includes(this.searchCriteria.serialNumber.toLowerCase())) &&
      (!this.searchCriteria.type || device.type === this.searchCriteria.type)
    );

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


  deleteDevice(): void {
    if (this.selectedDevice) {
      this.deviceService.delete(this.selectedDevice.id!).subscribe(() => {
        this.devices = this.devices.filter(device => device.id !== this.selectedDevice?.id);
        this.filteredDevices = this.filteredDevices.filter(device => device.id !== this.selectedDevice?.id);
        this.selectedDevice = null;
        this.operations = [];
      });
    }
  }

  exportToExcel(): void {
    //show message `file download` dialog
    this.messageService.add({ severity: 'info', summary: 'جاري التحميل', detail: 'جاري تحميل ملف Excel...' });

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

  importDevices(): void {
    const fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.accept = '.xlsx, .xls';
    fileInput.onchange = (event: any) => {
      const file = event.target.files[0];
      const reader = new FileReader();
      reader.onload = (e: any) => {
        const data = new Uint8Array(e.target.result);
        const workbook = XLSX.read(data, { type: 'array' });
        const worksheet = workbook.Sheets[workbook.SheetNames[0]];
        const jsonData = XLSX.utils.sheet_to_json(worksheet);

        // Map Excel data to DevicesDto
        // const devices: DevicesDto[] = jsonData.map((row: any) => ({
        //   id: null, // ID should be generated by backend
        //   laptopName: row['اسم اللاب توب'] || '',
        //   serialNumber: row['الرقم التسلسلي'] || '',
        //   type: row['النوع'] || '',
        //   source: row['الجهة'] || '',
        //   brotherName: row['اسم الأخ'] || '',
        //   systemPassword: row['كلمة مرور النظام'] || '',
        //   windowsPassword: row['كلمة مرور ويندوز'] || '',
        //   hardDrivePassword: row['كلمة التشفير'] || '',
        //   freezePassword: row['كلمة التجميد'] || '',
        //   code: row['الكود'] || '',
        //   card: row['الكرت'] || '',
        //   createdAt: row['تاريخ الإنشاء'] || new Date().toISOString(),
        // }));

        // Send to DeviceService
        // this.deviceService.importDevices(devices).subscribe({
        //   next: (newDevices) => {
        //     this.devices = [...this.devices, ...newDevices];
        //     this.filteredDevices = [...this.devices];
        //     this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم استيراد الأجهزة بنجاح!' });
        //   },
        //   error: (err) => {
        //     console.error('Error importing devices:', err);
        //     this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل استيراد الأجهزة.' });
        //   }
        // });
      };
      reader.readAsArrayBuffer(file);
    };
    fileInput.click();
  }


  toggleDarkMode(): void {
    this.darkMode = !this.darkMode;
  }

  addOperation(deviceId: number): void {
    const device = this.devices.find(d => d.id === deviceId);
    if (!device) {
      this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'الجهاز غير موجود!' });
      return;
    }

    // Open modal for adding operation
    this.dialogRef = this.dialogService.open(AddOperationDialogComponent, {
      header: 'إضافة عملية جديدة',
      width: '35%',
      contentStyle: { 'direction': 'rtl', 'padding': '1rem' },
      data: { deviceId, deviceName: device.laptopName }
    });

    // Handle modal close
    this.dialogRef.onClose.subscribe((operationData: Partial<CreateOperation>) => {
      if (operationData) {
        const newOperation: CreateOperation = {
          deviceId,
          operationName: operationData.operationName!,
          oldValue: operationData.oldValue || null,
          newValue: operationData.newValue || null,
          comment: operationData.comment || null
        };

        this.operationService.addOperation(newOperation).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم إضافة العملية بنجاح!' });
            if (this.selectedDevice?.id === deviceId) {
              this.deviceService.getById(deviceId).subscribe((response) => {
                this.operations = response.operationsDtos;
              });
            }
          },
          error: (err) => {
            console.error('Error adding operation:', err);
            this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل إضافة العملية. حاول مرة أخرى.' });
          }
        });
      }
    });
  }

  ngOnDestroy(): void {
    if (this.dialogRef) {
      this.dialogRef.close();
      this.dialogRef = null;
    }
  }

}