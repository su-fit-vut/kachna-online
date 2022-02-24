// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

import { IEnvironment, IEnvironmentParams } from "./ienvironment";

/**
 * Environment parameters modifiable by the current environment.
 */
const params: IEnvironmentParams = {
  production: false,
  baseApiUrl: 'https://localhost:5001/kachna/api',
  baseApiUrlDomain: 'localhost:5001',
};

// Set environment for the application.
export const environment: IEnvironment = new IEnvironment(params);

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
