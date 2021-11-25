// environment.prod.ts
// Author: David Chocholatý

import { IEnvironment, IEnvironmentParams } from "./ienvironment";

/**
 * Environment parameters modifiable by the current environment.
 */
const params: IEnvironmentParams = {
  production: true,
  baseApiUrl: 'kachna/api', // FIXME: Add real URL.
  baseApiUrlDomain: 'localhost:5000', // FIXME: Add real URL.
};

// Set environment for the application.
export const environment: IEnvironment = new IEnvironment(params);
