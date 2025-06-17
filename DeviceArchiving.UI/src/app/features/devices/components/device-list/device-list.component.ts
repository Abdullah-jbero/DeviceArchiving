import { Component, OnInit, OnDestroy, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import * as XLSX from 'xlsx';
import { DeviceService } from '../../../../core/services/device.service';
import { OperationService } from '../../../../core/services/operation.service';
import { CheckDuplicateDto, DevicesDto, DeviceUploadDto, DuplicateCheckResponse, ExcelDevice, OperationDto, SearchCriteria } from '../../../../core/models/device.model';
import { CreateOperation } from '../../../../core/models/operation.model';
import { AddOperationDialogComponent } from '../../../operations/components/add-operation-dialog/add-operation-dialog.component';
import { OperationListComponent } from '../../../operations/components/operation-list/operation-list.component';
import { FileSelectEvent } from 'primeng/fileupload';
import { BaseResponse } from '../../../../core/models/update-device.model';
import { CreateDeviceDto } from '../../../../core/models/create-device.model';
import { AccountService } from '../../../../core/services/account.service';
import { Route } from '@angular/router';



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

  displayDialog = false;
  excelData: ExcelDevice[] = [];


  constructor(
    private readonly deviceService: DeviceService,
    private readonly operationService: OperationService,
    private readonly dialogService: DialogService,
    private readonly accountService: AccountService,
    private readonly messageService: MessageService,
    private cdr: ChangeDetectorRef
  ) { }


  ngOnInit(): void {
    this.accountService.getUserInfo();
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
          this.devices = devices.filter(d => d.isActive === true);
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


  onFileSelected(event: any): void {
    const input = event;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.readExcelFile();
    } else {
      this.showWarning('لم يتم اختيار أي ملف.');
    }
  }

  private readExcelFile(): void {
    if (!this.validateFile()) return;

    this.loading = true;
    const reader = new FileReader();
    reader.onload = (e: any) => {
      const data = new Uint8Array(e.target.result);
      const devices = this.parseExcelData(data);

      if (devices.length === 0) {
        return;
      }

      // Check for duplicates and empty fields in file; stop if issues are found
      if (!this.checkDuplicatesInFile(devices)) {
        this.loading = false;
        this.selectedFile = null;
        this.excelData = [];
        return;
      }

      // Proceed to database check if no issues in file
      this.checkDuplicatesInDatabase(devices);
    };
    reader.onerror = () => {
      this.showError('حدث خطأ أثناء قراءة الملف');
      this.loading = false;
    };
    reader.readAsArrayBuffer(this.selectedFile!);
  }

  private validateFile(): boolean {
    if (!this.selectedFile) {
      this.showWarning('يرجى اختيار ملف Excel أولاً');
      return false;
    }

    const validMimeTypes = [
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      'application/vnd.ms-excel'
    ];
    const validExtensions = ['.xlsx', '.xls'];
    const fileExtension = this.selectedFile.name.toLowerCase().slice(this.selectedFile.name.lastIndexOf('.'));

    if (!validMimeTypes.includes(this.selectedFile.type) || !validExtensions.includes(fileExtension)) {
      this.showError('الملف غير صالح. يرجى اختيار ملف Excel (.xlsx أو .xls)');
      this.selectedFile = null;
      return false;
    }

    return true;
  }

  private parseExcelData(data: Uint8Array): ExcelDevice[] {
    const workbook = XLSX.read(data, { type: 'array' });
    const worksheet = workbook.Sheets[workbook.SheetNames[0]];
    const rows = XLSX.utils.sheet_to_json(worksheet, { header: 1, blankrows: false });

    // Validate headers
    const headers = rows[0] as string[];
    const expectedHeaders = ['الرقم', 'الجهة', 'اسم الأخ', 'اسم اللابتوب', 'كلمة سر النظام', 'كلمة سر الويندوز', 'كلمة سر الهارد', 'كلمة التجميد', 'الكود', 'النوع', 'رقم السيريال', 'الكرت', 'ملاحظات', 'التاريخ', 'رقم التواصل'];
    if (!headers.every((h, i) => h === expectedHeaders[i])) {
      this.showError('رأس الجدول غير صحيح. يرجى استخدام القالب الصحيح: ' + expectedHeaders.join(', '));
      return [];
    }

    const devices: ExcelDevice[] = [];
    rows.slice(1).forEach((row: any) => {
      const device: ExcelDevice = {
        source: row[1] != null ? row[1].toString().trim() : '',
        brotherName: row[2] != null ? row[2].toString().trim() : '',
        laptopName: row[3] != null ? row[3].toString().trim() : '',
        systemPassword: row[4] != null ? row[4].toString().trim() : '',
        windowsPassword: row[5] != null ? row[5].toString().trim() : '',
        hardDrivePassword: row[6] != null ? row[6].toString().trim() : '',
        freezePassword: row[7] != null ? row[7].toString().trim() : '',
        code: row[8] != null ? row[8].toString().trim() : '',
        type: row[9] != null ? row[9].toString().trim() : '',
        serialNumber: row[10] != null ? row[10].toString().trim() : '',
        card: row[11] != null ? row[11].toString().trim() : '',
        comment: row[12] != null ? row[12].toString().trim() || null : null,
        contactNumber: row[14] != null ? row[14].toString().trim() : '',
        isSelected: true,
        isDuplicateSerial: false,
        isDuplicateLaptopName: false
      };
      devices.push(device);
    });

    if (devices.length === 0) {
      this.showWarning('لم يتم العثور على بيانات صالحة في الملف');
      this.loading = false;
    }
    return devices;
  }

  private checkDuplicatesInFile(devices: ExcelDevice[]): boolean {
    const serialNumbers = new Set<string>();
    const laptopNames = new Set<string>();
    const duplicateSerials = new Set<string>();
    const duplicateLaptopNames = new Set<string>();
    const columnErrors: { row: number; fields: string[] }[] = [];

    // Check for empty required fields and duplicates
    devices.forEach((device, index) => {
      const rowErrors: string[] = [];
      const rowNumber = index + 2; // +2 for header row and 1-based indexing

      // Validate required fields
      const requiredFields: { key: keyof ExcelDevice; label: string }[] = [
        { key: 'source', label: 'الجهة' },
        { key: 'brotherName', label: 'اسم الأخ' },
        { key: 'laptopName', label: 'اسم اللابتوب' },
        { key: 'systemPassword', label: 'كلمة سر النظام' },
        { key: 'windowsPassword', label: 'كلمة سر الويندوز' },
        { key: 'hardDrivePassword', label: 'كلمة سر الهارد' },
        { key: 'freezePassword', label: 'كلمة التجميد' },
        { key: 'code', label: 'الكود' },
        { key: 'type', label: 'النوع' },
        { key: 'serialNumber', label: 'رقم السيريال' },
        { key: 'card', label: 'الكرت' },

      ];

      requiredFields.forEach(field => {
        const value = device[field.key];
        // Check if the value is a string before using trim()
        if (typeof value === 'string' && value.trim() === '') {
          rowErrors.push(field.label);
        }
      });

      // Check for duplicates
      if (device.serialNumber) {
        if (serialNumbers.has(device.serialNumber)) {
          duplicateSerials.add(device.serialNumber);
        } else {
          serialNumbers.add(device.serialNumber);
        }
      }

      if (device.laptopName) {
        if (laptopNames.has(device.laptopName)) {
          duplicateLaptopNames.add(device.laptopName);
        } else {
          laptopNames.add(device.laptopName);
        }
      }

      if (rowErrors.length > 0) {
        columnErrors.push({ row: rowNumber, fields: rowErrors });
      }
    });

    // Construct error message
    const errorMessages: string[] = [];


    if (columnErrors.length > 0) {
      const emptyFieldErrors = columnErrors.map(e =>
        `الصف ${e.row}: ${e.fields.join('، ')}`
      );
      errorMessages.push(`حقول مطلوبة فارغة: ${emptyFieldErrors.join('؛ ')}`);
    }


    if (duplicateSerials.size > 0) {
      errorMessages.push(`أرقام سيريال مكررة: ${Array.from(duplicateSerials).join(', ')}`);
    }
    if (duplicateLaptopNames.size > 0) {
      errorMessages.push(`أسماء لابتوب مكررة: ${Array.from(duplicateLaptopNames).join(', ')}`);
    }

    if (errorMessages.length > 0) {
      this.showError(
        `الملف يحتوي على أخطاء. يرجى تصحيح الملف وإعادة التحميل. `
      );

      this.showError(
        `${errorMessages.join('، ')} `
      );
      this.showError(
        `تأكد من أن جميع حقول مطلوبة مملوءة وأن أرقام السيريال وأسماء اللابتوب فريدة.`
      );

      return false;
    }

    // No issues found
    return true;
  }

  private checkDuplicatesInDatabase(devices: ExcelDevice[]): void {
    const checkItems: CheckDuplicateDto[] = devices.map(device => ({
      serialNumber: device.serialNumber,
      laptopName: device.laptopName
    }));

    this.deviceService.checkDuplicates(checkItems)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: BaseResponse<DuplicateCheckResponse>) => {
          if (response.success && response.data) {
            devices.forEach(device => {
              if (response.data.duplicateSerialNumbers.includes(device.serialNumber)) {
                device.isDuplicateSerial = true;
                device.isSelected = false;
              }
              if (response.data.duplicateLaptopNames.includes(device.laptopName)) {
                device.isDuplicateLaptopName = true;
                device.isSelected = false;
              }
            });

          }

          // Sort duplicates first (SerialNumber duplicates, then LaptopName duplicates, then others)
          this.excelData = devices.sort((a, b) => {
            if (a.isDuplicateSerial && !b.isDuplicateSerial) return -1;
            if (!a.isDuplicateSerial && b.isDuplicateSerial) return 1;
            if (a.isDuplicateLaptopName && !b.isDuplicateLaptopName) return -1;
            if (!a.isDuplicateLaptopName && b.isDuplicateLaptopName) return 1;
            return 0;
          });

          this.displayDialog = true;
          this.loading = false;
        },
        error: (err: Error) => {
          this.showError(err.message || 'حدث خطأ أثناء التحقق من التكرارات');
          this.loading = false;
        }
      });
  }

  private showWarning(message: string): void {
    this.messageService.add({
      severity: 'warn',
      summary: 'تحذير',
      detail: message,
      life: 5000
    });
  }

  private showError(message: string): void {
    this.messageService.add({
      severity: 'error',
      summary: 'خطأ',
      detail: message,
      life: 10000 // Longer duration for error to ensure user sees it
    });
  }


  uploadSelectedDevices(): void {
    const selectedDevices: DeviceUploadDto[] = this.excelData
      .filter(device => device.isSelected)
      .map(device => ({
        source: device.source,
        brotherName: device.brotherName,
        laptopName: device.laptopName,
        systemPassword: device.systemPassword,
        windowsPassword: device.windowsPassword,
        hardDrivePassword: device.hardDrivePassword,
        freezePassword: device.freezePassword,
        code: device.code,
        type: device.type,
        serialNumber: device.serialNumber,
        card: device.card,
        comment: device.comment,
        contactNumber: device.contactNumber,
        isUpdate: device.isDuplicateSerial || device.isDuplicateLaptopName,

      }));

    if (selectedDevices.length === 0) {
      this.messageService.add({
        severity: 'warn',
        summary: 'تحذير',
        detail: 'لم يتم تحديد أي أجهزة للرفع',
        life: 5000
      });
      this.displayDialog = false;
      return;
    }

    this.loading = true;
    this.deviceService.uploadDevices(selectedDevices)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: BaseResponse<number>) => {
          if (response.success) {
            this.messageService.add({
              severity: 'success',
              summary: 'نجاح',
              detail: response.message || `تم معالجة ${response.data} جهاز${response.data === 1 ? '' : ' أجهزة'} بنجاح`,
              life: 5000
            });
            this.selectedFile = null;
            this.excelData = [];
            this.loadDevices();
          } else {
            this.messageService.add({
              severity: 'error',
              summary: 'خطأ',
              detail: response.message || 'فشل معالجة الأجهزة',
              life: 5000
            });
          }
          this.loading = false;
          this.displayDialog = false;
        },
        error: (err: Error) => {
          this.loading = false;
          this.selectedFile = null;
          this.excelData = [];
          this.messageService.add({
            severity: 'error',
            summary: 'خطأ',
            detail: err.message || 'حدث خطأ أثناء معالجة الأجهزة',
            life: 5000
          });
          this.displayDialog = false;
        }
      });
  }

  selectAll(): void {
    if (this.loading) return;

    this.excelData = this.excelData.map(device => ({
      ...device,
      isSelected: true
    }));


    this.cdr.detectChanges();
  }

  // Deselect all devices
  deselectAll(): void {
    if (this.loading) return; // Prevent action during loading

    this.excelData = this.excelData.map(device => ({
      ...device,
      isSelected: false
    }));

    this.cdr.detectChanges(); // Trigger change detection
  }

  onCheckboxChange(event: Event, device: any): void {
    const input = event.target as HTMLInputElement;
    device.isSelected = input.checked;
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
