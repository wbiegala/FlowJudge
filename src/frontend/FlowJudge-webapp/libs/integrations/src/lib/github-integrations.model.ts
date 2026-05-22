export interface ConnectIntegrationResponse {
  redirectUrl: string;
}

export interface GetGitHubInstallationRepositoriesResponseItem {
  id: number;
  name: string;
  fullName: string;
}

export interface CommitGitHubIntegrationInstallationRequest {
  name: string;
  repositoriesConfiguration: Array<GitHubRepositoryConfiguration>;
}

export interface GitHubRepositoryConfiguration {
  githubId: number;
  track: boolean;
}
