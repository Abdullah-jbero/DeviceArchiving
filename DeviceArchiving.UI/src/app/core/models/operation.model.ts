export interface Operation {
    deviceId: number;
    operationName: string;
    oldValue?: string | null;
    newValue?: string | null;
    comment?: string | null;
    createdAt: string;

}

export interface CreateOperation {
    deviceId: number;
    operationName: string;
    oldValue: string | null;
    newValue: string | null;
    comment: string | null;
}