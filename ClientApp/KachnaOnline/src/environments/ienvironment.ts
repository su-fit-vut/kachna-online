// ienvironment.ts
// Author: David Chocholatý

/**
 * Environment accessible from anywhere in the application.
 */
export class IEnvironment implements IEnvironmentParams {
  // Modifiable by the current environment.
  public production: boolean = false;
  public baseApiUrl: string = 'http://localhost:5000';
  public baseApiUrlDomain: string = 'localhost:5000';

  // Global constants.
  public siteName: string = 'Kachna Online';
  public accessTokenStorageName: string = 'accessToken';
  public kisAccessTokenStorageName: string = 'kisAccessToken';
  public kisApiUrl: string = 'https://su-int.fit.vutbr.cz/kis/api';
  public kisApiUrlDomain: string = 'su-int.fit.vutbr.cz';

  /**
   * Change default values based on environment settings.
   * @param params Values set by environment.
   */
  constructor(params: IEnvironmentParams) {
    this.production = params.production ?? this.production;
    this.baseApiUrl = params.baseApiUrl ?? this.baseApiUrl;
    this.baseApiUrlDomain = params.baseApiUrlDomain ?? this.baseApiUrlDomain;
  }
}

/**
 * Parameters modifiable by environment (development, production etc.).
 */
export interface IEnvironmentParams {
  production: boolean;
  baseApiUrl?: string;
  baseApiUrlDomain?: string;
}
