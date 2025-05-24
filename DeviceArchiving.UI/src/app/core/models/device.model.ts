

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
  comment?: string;
  contactNumber?: string;
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