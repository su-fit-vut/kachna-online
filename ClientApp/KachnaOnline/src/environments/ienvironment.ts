// ienvironment.ts
// Author: David Chocholatý

import { LocalTokenContent } from "../app/models/users/auth/local-token-content.model";
import { KisTokenContent } from "../app/models/users/auth/kis/kis-token-content.model";
import { KisLoggedInUserInformation } from "../app/models/users/kis-logged-in-user-information.model";

/**
 * Environment accessible from anywhere in the application.
 */
export class IEnvironment implements IEnvironmentParams {
  // Modifiable by the current environment.
  public production: boolean = false;
  public baseApiUrl: string = 'http://localhost:5000/kachna/api';
  public baseApiUrlDomain: string = 'localhost:5000';

  // Global constants.
  public siteName: string = 'Kachna Online';
  public readonly accessTokenStorageName: string = 'accessToken';
  public readonly kisAccessTokenStorageName: string = 'kisAccessToken';
  public readonly kisRefreshTokenStorageName: string = 'kisRefreshToken';
  public readonly userDataStorageName: string = 'userData';
  public readonly returnAddressStorageName: string = 'returnAddress';
  public readonly localTokenContentStorageName: string = 'localTokenContent';
  public readonly kisTokenContentStorageName: string = 'kisTokenContent';
  public readonly kisLoggedInUserInformationStorageName: string = 'kisLoggedInUserInformationStorageName';
  public readonly homePageViewStorageName: string ='homePageViewStorageName';
  public kisApiUrl: string = 'https://su-int.fit.vutbr.cz/kis/api';
  public kisApiUrlDomain: string = 'su-int.fit.vutbr.cz';
  public readonly datePattern: string = "^(?:(?:(?:0?[1-9]|1\\d|2[0-8]). ?(?:0?[1-9]|1[0-2])|(?:29|30). ?(?:0?[13-9]|1[0-2])|31. ?(?:0?[13578]|1[02])). ?\\d{4}|29. ?0?2. ?(?:\\d\\d(?:0[48]|[2468][048]|[13579][26])|(?:[02468][048]|[13579][26])00))$";

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
