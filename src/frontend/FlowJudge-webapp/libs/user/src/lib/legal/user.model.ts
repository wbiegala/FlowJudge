export interface GetUserLegalStateResponse {
  isLegal: boolean;
  missings: Array<UserLegalMissings>;
}

export type UserLegalMissings = 'EmailConfirmed' | 'TermsAndConditionsActualVersionAccepted' | 'PrivacyPolicyActualVersionAccepted';
