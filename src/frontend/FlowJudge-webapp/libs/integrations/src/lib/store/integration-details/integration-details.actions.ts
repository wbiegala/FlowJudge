export class InitializeEditIntegration {
  static readonly type = '[Integration] Initializes edit integration form';
  constructor(public integrationId: string) {}
}

export class EnableTrackingForRepository {
  static readonly type = '[Integration] Enables tracking for repository';
  constructor(public repositoryId: string) {}
}

export class DisableTrackingForRepository {
  static readonly type = '[Integration] Disables tracking for repository';
  constructor(public repositoryId: string) {}
}
