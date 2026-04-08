export interface GetDocumentVersionResponse {
  kind: DocumentVersionKind;
  versionId: string;
  versionNumber: number;
  content: string;
}

export type DocumentVersionKind = 'TermsAndConditions' | 'PrivacyPolicy';
