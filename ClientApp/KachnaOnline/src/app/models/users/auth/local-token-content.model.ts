// local-token-content.model.ts
// Author: David Chocholatý

/**
 * Token content of Kachna Online API access token.
 */
export class LocalTokenContent {
  sub: string = "";
  email: string = "";
  given_name: string = "";
  krt: string = ""; // KIS refresh token.
  role: string[] = [];
  nbf: number = -1;
  exp: number = -1;
  iat: number = -1;
}
