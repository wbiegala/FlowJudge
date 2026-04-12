export class StartLogin {
  static readonly type = '[User] Start login';
}

export class Authenticate {
  static readonly type = '[User] Authenticate';
  constructor(public accessToken: string, public identityToken: string){}
}

export class SetAuthenticatedUserContext {
  static readonly type = '[User] Set user context';
}

export class TryRestoreAuthenticatedUserContext {
  static readonly type = '[User] Try restore user context';
}

export class StartLogout {
  static readonly type = '[user] Start logout';
}

export class ClearAuthenticatedUserContext {
  static readonly type = '[User] Clear user context';
}
