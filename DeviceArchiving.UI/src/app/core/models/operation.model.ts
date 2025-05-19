export interface Operation {
    deviceId: number;
    operationName: string;
    oldValue?: string | null;
    newValue?: string | null;
    createdAt: string;
    comment?: string;
}