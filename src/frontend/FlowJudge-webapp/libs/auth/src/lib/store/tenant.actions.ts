export class StartLogin {
  static readonly type = '[tenant] Start login';
}

export class Authenticate {
  static readonly type = '[tenant] Authenticate';
  constructor(public accessToken: string, public identityToken: string){}
}

export class SetTenantContext {
  static readonly type = '[tenant] Set tenant context';
}

export class TryRestoreTenantContext {
  static readonly type = '[tenant] Try restore tenant context';
}

export class ClearTenantContext {
  static readonly type = '[tenant] Clear tenant context';
}
