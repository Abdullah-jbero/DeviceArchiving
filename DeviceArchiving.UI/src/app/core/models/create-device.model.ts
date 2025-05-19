

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
    Comment: string | null;
    contactNumber: string | null;
    card: string;
}

