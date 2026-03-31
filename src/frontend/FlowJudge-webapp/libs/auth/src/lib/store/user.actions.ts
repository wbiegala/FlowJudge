export class StartLogin {
  static readonly type = '[User] Start login';
}

export class Authenticate {
  static readonly type = '[User] Authenticate';
  constructor(public accessToken: string, public identityToken: string){}
}

export class SetUserContext {
  static readonly type = '[User] Set user context';
}

export class TryRestoreUserContext {
  static readonly type = '[User] Try restore user context';
}

export class ClearUserContext {
  static readonly type = '[User] Clear user context';
}
