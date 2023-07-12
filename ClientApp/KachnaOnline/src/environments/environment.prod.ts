import { IEnvironment, IEnvironmentParams } from "./ienvironment";

/**
 * Environment parameters modifiable by the current environment.
 */
const params: IEnvironmentParams = {
  production: true,
  baseApiUrl: 'https://su.fit.vut.cz/kachna/api',
  baseApiUrlDomain: 'su.fit.vut.cz',
};

// Set environment for the application.
export const environment: IEnvironment = new IEnvironment(params);
