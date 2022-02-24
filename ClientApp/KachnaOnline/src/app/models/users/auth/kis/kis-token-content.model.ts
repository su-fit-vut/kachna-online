/**
 * KIS token content of KIS API access token.
 */
export class KisTokenContent {
  iss: string = "";
  iat: number = -1;
  exp: number = -1;
  sub: string = "";
  scp: string = "";
}
