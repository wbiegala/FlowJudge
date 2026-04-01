export interface TokenPairResponse {
  accessToken: string;
  identityToken: string;
}

export interface GetUserDataResponse {
  id: string;
  username: string;
  email: string;
}

export interface LogoutRequest {
  identityToken: string;
  uiContext: string;
}

export interface LogoutResponse {
  logoutRedirectUrl: string;
}
