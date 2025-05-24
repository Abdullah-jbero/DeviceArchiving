import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import * as XLSX from 'xlsx';
import { DeviceService } from '../../../../core/services/device.service';
import { OperationService } from '../../../../core/services/operation.service';
import { DevicesDto, OperationDto } from '../../../../core/models/device.model';
import { CreateOperation } from '../../../../core/models/operation.model';
import { AddOperationDialogComponent } from '../../../operations/components/add-operation-dialog/add-operation-dialog.component';
import { OperationListComponent } from '../../../operations/components/operation-list/operation-list.component';
import { FileSelectEvent } from 'primeng/fileupload';
import { BaseResponse } from '../../../../core/models/update-device.model';

interface SearchCriteria {
  laptopName: string;
  serialNumber: string;
  type: string;
}

interface Filter {
  value: string | null;
  matchMode: string;
}

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.css'],
  standalone: false,
  providers: [DialogService, MessageService],
})
export class DeviceListComponent implements OnInit, OnDestroy {
  devices: DevicesDto[] = [];
  filteredDevices: DevicesDto[] = [];
  selectedDevice: DevicesDto | null = null;
  operations: OperationDto[] = [];
  globalSearchQuery: string = '';
  searchCriteria: SearchCriteria = {
    laptopName: '',
    serialNumber: '',
    type: '',
  };
  deviceTypes: { label: string; value: string }[] = [];
  dialogRef: DynamicDialogRef | null = null;
  loading: boolean = false;
  selectedFile: File | null = null;
  private destroy$ = new Subject<void>();
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  private readonly searchableFields: (keyof DevicesDto)[] = [
    'source',
    'laptopName',
    'brotherName',
    'systemPassword',
    'windowsPassword',
    'hardDrivePassword',
    'freezePassword',
    'serialNumber',
    'type',
    'code',
    'comment',
    'contactNumber',
    'userName',
    'createdAt',

  ];

  constructor(
    private readonly deviceService: DeviceService,
    private readonly operationService: OperationService,
    private readonly dialogService: DialogService,
    private readonly messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.loadDevices();
  }

  private loadDevices(): void {
    this.loading = true;
    this.deviceService
      .getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (devices: DevicesDto[]) => {
          this.devices = devices;
          this.filteredDevices = [...devices];
          this.deviceTypes = [
            ...new Set(devices.map((device) => device.type)),
          ].map((type) => ({ label: type, value: type }));
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: 'خطأ',
            detail: 'فشل تحميل الأجهزة.',
          });
        },
      });
  }

  selectDevice(device: DevicesDto): void {
    this.selectedDevice = device;
  }

  showOperations(device: DevicesDto): void {
    this.deviceService
      .getById(device.id!)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.operations = response.operationsDtos.map((op: OperationDto) => ({
            operationName: op.operationName,
            oldValue: op.oldValue || '',
            newValue: op.newValue || '',
            comment: op.comment || '',
            userName: op.userName || '',
            createdAt: op.createdAt,
          }));
          this.dialogRef = this.dialogService.open(OperationListComponent, {
            header: 'العمليات',
            width: '70%',
            contentStyle: { direction: 'rtl', padding: '1rem' },
            data: { operations: this.operations },
          });
        },
        error: (error) => {
          console.error('Error fetching operations:', error);
          this.messageService.add({
            severity: 'error',
            summary: 'خطأ',
            detail: 'فشل جلب العمليات. حاول مرة أخرى.',
          });
        },
      });
  }

  applyFilter(): void {
    let filtered = [...this.devices];
    if (this.globalSearchQuery.trim()) {
      const query = this.globalSearchQuery.toLowerCase();
      filtered = filtered.filter((device) =>
        this.searchableFields.some((key) =>
          (device[key] as string)?.toLowerCase().includes(query)
        )
      );
    }

    filtered = filtered.filter(
      (device) =>
        (!this.searchCriteria.laptopName ||
          device.laptopName
            .toLowerCase()
            .includes(this.searchCriteria.laptopName.toLowerCase())) &&
        (!this.searchCriteria.serialNumber ||
          device.serialNumber
            .toLowerCase()
            .includes(this.searchCriteria.serialNumber.toLowerCase())) &&
        (!this.searchCriteria.type || device.type === this.searchCriteria.type)
    );

    this.filteredDevices = filtered;
  }

  clearSearch(): void {
    this.globalSearchQuery = '';
    this.searchCriteria = { laptopName: '', serialNumber: '', type: '' };
    this.filteredDevices = [...this.devices];
  }

  deleteDevice(): void {
    if (!this.selectedDevice?.id) return;

    this.deviceService
      .delete(this.selectedDevice.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.devices = this.devices.filter(
            (device) => device.id !== this.selectedDevice?.id
          );
          this.filteredDevices = this.filteredDevices.filter(
            (device) => device.id !== this.selectedDevice?.id
          );
          this.selectedDevice = null;
          this.operations = [];
          this.messageService.add({
            severity: 'success',
            summary: 'نجاح',
            detail: 'تم حذف الجهاز بنجاح.',
          });
        },
        error: () => {
          this.messageService.add({
            severity: 'error',
            summary: 'خطأ',
            detail: 'فشل حذف الجهاز. حاول مرة أخرى.',
          });
        },
      });
  }

  exportToExcel(): void {
    this.messageService.add({
      severity: 'info',
      summary: 'جاري التحميل',
      detail: 'جاري تحميل ملف Excel...',
    });

    const data = this.filteredDevices.map((device) => ({
      الجهة: device.source,
      'اسم اللاب توب': device.laptopName,
      'اسم الأخ': device.brotherName,
      'كلمة مرور النظام': device.systemPassword,
      'كلمة مرور ويندوز': device.windowsPassword,
      'كلمة التشفير': device.hardDrivePassword,
      'كلمة التجميد': device.freezePassword,
      'الرقم التسلسلي': device.serialNumber,
      'النوع': device.type,
      'الكود': device.code,
      'الكرت': device.card,
      'ملاحظة': device.comment,
      'رقم التواصل': device.contactNumber,
      'تم بواسطة': device.userName,
      'تاريخ الإنشاء': device.createdAt.toString(),
    }));

    const worksheet = XLSX.utils.json_to_sheet(data);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'الأجهزة');

    const excelBuffer = XLSX.write(workbook, {
      bookType: 'xlsx',
      type: 'array',
    });
    const blob = new Blob([excelBuffer], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'devices.xlsx';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  onFileSelected(event: FileSelectEvent): void {
    const input = event;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.uploadFile();
    } else {
      this.messageService.add({
        severity: 'warn',
        summary: 'تحذير',
        detail: 'لم يتم اختيار أي ملف.'
      });
    }
  }

  uploadFile(): void {
    if (!this.selectedFile) {
      this.messageService.add({
        severity: 'warn',
        summary: 'تحذير',
        detail: 'يرجى اختيار ملف Excel أولاً'
      });
      return;
    }

    const validMimeTypes = [
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', // .xlsx
      'application/vnd.ms-excel' // .xls
    ];
    const validExtensions = ['.xlsx', '.xls'];
    const fileExtension = this.selectedFile.name.toLowerCase().slice(this.selectedFile.name.lastIndexOf('.'));

    if (!validMimeTypes.includes(this.selectedFile.type) || !validExtensions.includes(fileExtension)) {
      this.messageService.add({
        severity: 'error',
        summary: 'خطأ',
        detail: 'الملف غير صالح. يرجى اختيار ملف Excel (.xlsx أو .xls)'
      });
      this.selectedFile = null;
      return;
    }

    this.loading = true;
    const formData = new FormData();
    formData.append('file', this.selectedFile);

    this.deviceService.uploadFile(formData)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: BaseResponse<number>) => {
          if (response.success) {
            this.messageService.add({
              severity: 'success',
              summary: 'نجاح',
              detail: response.message || `تم رفع الملف بنجاح. تمت إضافة ${response.data} جهاز${response.data === 1 ? '' : ' أجهزة'}.`
            });
            this.selectedFile = null;
            this.loadDevices();
          } else {
            this.messageService.add({
              severity: 'error',
              summary: 'خطأ',
              detail: response.message || 'فشل رفع الملف',
              life: 10000
            });
          }
          this.loading = false;
        },
        error: (err: Error) => {
          this.loading = false;
          this.selectedFile = null;
          this.messageService.add({
            severity: 'error',
            summary: 'خطأ',
            detail: err.message || 'حدث خطأ أثناء رفع الملف'
          });
        }
      });
  }


  triggerFileSelect(): void {
    if (this.fileInput) {
      this.fileInput.nativeElement.click();
    }
  }
  addOperation(deviceId: number): void {
    const device = this.devices.find((d) => d.id === deviceId);
    if (!device) {
      this.messageService.add({
        severity: 'error',
        summary: 'خطأ',
        detail: 'الجهاز غير موجود!',
      });
      return;
    }

    this.dialogRef = this.dialogService.open(AddOperationDialogComponent, {
      header: 'إضافة عملية جديدة',
      width: '35%',
      contentStyle: { direction: 'rtl', padding: '1rem' },
      data: { deviceId, deviceName: device.laptopName },
    });

    this.dialogRef.onClose
      .pipe(takeUntil(this.destroy$))
      .subscribe((operationData: Partial<CreateOperation>) => {
        if (operationData) {
          const newOperation: CreateOperation = {
            deviceId,
            operationName: operationData.operationName!,
            oldValue: operationData.oldValue || null,
            newValue: operationData.newValue || null,
            comment: operationData.comment || null,
          };

          this.operationService
            .addOperation(newOperation)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
              next: () => {
                this.messageService.add({
                  severity: 'success',
                  summary: 'نجاح',
                  detail: 'تم إضافة العملية بنجاح!',
                });
                if (this.selectedDevice?.id === deviceId) {
                  this.deviceService
                    .getById(deviceId)
                    .pipe(takeUntil(this.destroy$))
                    .subscribe((response) => {
                      this.operations = response.operationsDtos;
                    });
                }
              },
              error: (err) => {
                console.error('Error adding operation:', err);
                this.messageService.add({
                  severity: 'error',
                  summary: 'خطأ',
                  detail: 'فشل إضافة العملية. حاول مرة أخرى.',
                });
              },
            });
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    if (this.dialogRef) {
      this.dialogRef.close();
      this.dialogRef = null;
    }
  }
}
