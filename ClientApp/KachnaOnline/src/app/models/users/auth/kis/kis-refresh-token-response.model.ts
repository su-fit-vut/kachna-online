/**
 * KIS response to refresh access token request to KIS API.
 */
export class KisRefreshTokenResponse {
  auth_token: string;
  expires_at: string;
  token_type: string;
  refresh_token: string;
}
