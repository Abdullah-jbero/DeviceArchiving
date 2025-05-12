export interface Device {
  id?: number; 
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
  isActive: boolean;
  createdAt: string;
}