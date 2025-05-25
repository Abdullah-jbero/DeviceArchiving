export interface AuthenticationResponse {
    token: string;
    userName: string;
    picture: string;
}

export interface AuthenticationRequest {
  email: string;
  password: string;
}
