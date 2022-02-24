import { IEnvironment, IEnvironmentParams } from "./ienvironment";

/**
 * Environment parameters modifiable by the current environment.
 */
const params: IEnvironmentParams = {
  production: true,
  baseApiUrl: 'https://www.su.fit.vutbr.cz/kachna/api',
  baseApiUrlDomain: 'su.fit.vutbr.cz',
};

// Set environment for the application.
export const environment: IEnvironment = new IEnvironment(params);
