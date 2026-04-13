export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  code?: string;
  [key: string]: unknown;
}

export interface AppProblem {
  status: number;
  code: string | null;
  type: string | null;
  title: string | null;
  detail: string | null;
  extensions: Record<string, unknown>;
  raw: ProblemDetails | null;
}
