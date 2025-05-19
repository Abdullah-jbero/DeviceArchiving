

export interface DevicesDto {
  id: number;
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
  comment?: string;
  contactNumber?: string;
  card: string;
  userName: string;
  createdAt: string;
}

export interface OperationDto {
  operationName: string;
  oldValue?: string;
  newValue?: string;
  comment?: string;
  createdAt: string;
  userName?: string; // Make optional for flexibility
}

export interface DeviceDto extends DevicesDto {
  operationsDtos: OperationDto[];
}