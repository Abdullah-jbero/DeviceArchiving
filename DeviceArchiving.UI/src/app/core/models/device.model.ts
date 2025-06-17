// create-device.model.ts (assumed)
export interface CreateDeviceDto {
  source: string;
  brotherName: string;
  laptopName: string;
  systemPassword: string;
  windowsPassword: string;
  hardDrivePassword: string;
  freezePassword: string;
  code: string;
  type: string;
  serialNumber: string;
  card: string;
  comment?: string | null;
  contactNumber?: string | null;
}

// device.model.ts
export interface DevicesDto {
  id: number;
  source: string;
  laptopName: string;
  brotherName: string;
  systemPassword: string;
  windowsPassword: string;
  hardDrivePassword: string;
  freezePassword: string;
  serialNumber: string;
  type: string;
  code: string;
  card: string;
  comment?: string | null; // Allow null to match CreateDeviceDto
  contactNumber?: string | null; // Allow null to match CreateDeviceDto
  userName: string;
  createdAt: string;
  isActive: boolean;
}

export interface OperationDto {
  operationName: string;
  oldValue?: string | null;
  newValue?: string | null;
  comment?: string | null;
  createdAt: string;
  userName?: string | null; // Already optional, kept as is
}

export interface DeviceDto extends DevicesDto {
  operationsDtos: OperationDto[];
}

export interface SearchCriteria {
  laptopName: string;
  serialNumber: string;
  type: string;
}

export interface ExcelDevice extends CreateDeviceDto {
  isSelected: boolean;
  isDuplicateSerial: boolean; // For SerialNumber duplicates (red styling)
  isDuplicateLaptopName: boolean; // For LaptopName duplicates (orange styling)
}

export interface CheckDuplicateDto {
  serialNumber: string; // Standardized to camelCase
  laptopName: string; // Standardized to camelCase
}

export interface DuplicateCheckResponse {
  duplicateSerialNumbers: string[];
  duplicateLaptopNames: string[];
}

export interface DeviceUploadDto extends CreateDeviceDto {
  isUpdate: boolean;
}