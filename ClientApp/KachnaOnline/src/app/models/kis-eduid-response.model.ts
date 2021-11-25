// kis-eduid-response.model.ts
// Author: David Chocholatý

/**
 * KIS response from GET kis/api/auth/eduid.
 */
export class KisEduIdResponse {
    session: string = "";
    wayf_url: string = "";
}
