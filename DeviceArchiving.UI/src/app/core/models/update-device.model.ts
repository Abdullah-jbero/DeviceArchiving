import { CreateDeviceDto } from "./create-device.model";


export interface UpdateDeviceDto extends CreateDeviceDto {

}

export interface AuthenticationResponse {
    token: string;
    userName: string;
    picture: string;
}