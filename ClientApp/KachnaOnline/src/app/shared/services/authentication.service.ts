// authentication.service.ts
// Author: David Chocholatý

import { environment } from '../../../environments/environment';
import { KisEduIdResponse } from '../../models/users/auth/kis/kis-eduid-response.model';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HTTP_INTERCEPTORS, HttpInterceptor } from '@angular/common/http';
import { Location } from '@angular/common';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LocalTokenContent } from "../../models/users/auth/local-token-content.model";
import { RoleTypes } from "../../models/users/auth/role-types.model";
import { User } from "../../models/users/user.model";
import { AccessTokens } from "../../models/users/auth/access-tokens.model";
import { KisTokenContent } from "../../models/users/auth/kis/kis-token-content.model";
import { KisLoggedInUserInformation } from "../../models/users/kis-logged-in-user-information.model";
import { KisRefreshTokenResponse } from "../../models/users/auth/kis/kis-refresh-token-response.model";
import { throwError } from "rxjs";

const AUTH_API = `${environment.baseApiUrl}/auth`;

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  constructor(
    private router: Router,
    private http: HttpClient,
    private toastr: ToastrService,
    private location: Location,
    public jwtHelper: JwtHelperService,
    private route: ActivatedRoute,
  ) { }

  localTokenContent: LocalTokenContent = new LocalTokenContent();
  kisTokenContent: KisTokenContent = new KisTokenContent();
  kisLoggedInUserInformation: KisLoggedInUserInformation = new KisLoggedInUserInformation();
  refreshLocalTokenIntervalHandle: number;

  user: User = new User();

  getSessionIdFromKisEduId() {
    let params = new HttpParams().set('redirect', `${window.location.origin}/login`)
    this.http.get<KisEduIdResponse>(`${environment.kisApiUrl}/auth/eduid`, { params: params }).toPromise()
      .then((res: KisEduIdResponse) => {
        let kisResponse = res;
        localStorage.setItem(environment.returnAddressStorageName, this.router.url);
        window.open(kisResponse.wayf_url, '_self');
      }).catch((error: any) => {
        this.toastr.error("Přihlášení se ke KIS selhalo.", "Autentizace");
        return throwError(error);
    });
  }

  logIn() {
    let sessionId: string = "";
    this.route.queryParams
    .subscribe(params => {
      sessionId = params['session'];
    });

    let params = new HttpParams().set('session', sessionId);
    this.http.get<AccessTokens>(`${AUTH_API}/accessTokenFromSession`, { params: params }).toPromise()
        .then((res) => {
          this.handleAccessTokens(res);

          this.refreshLocalTokenIntervalHandle = setInterval( () => {
              this.refreshAuthToken();
            },
            2500 * 1000); // FIXME: Refresh according to expire value returned from KO API.
        }).catch((error: any) => {
          this.toastr.error("Načtení přístupových údajů selhalo.", "Autentizace");
          return throwError(error);
    });
  }

  private handleAccessTokens(res: AccessTokens) {
    localStorage.setItem(environment.accessTokenStorageName, res.accessToken);
    localStorage.setItem(environment.kisAccessTokenStorageName, res.kisAccessToken);
    this.decodeLocalToken();
    this.decodeKisToken();
    this.getInformationAboutUser();
  }

  logOut() {
    localStorage.removeItem(environment.accessTokenStorageName);
    localStorage.removeItem(environment.kisAccessTokenStorageName);
    localStorage.removeItem(environment.userDataStorageName);
    localStorage.removeItem(environment.kisRefreshTokenStorageName);
    localStorage.removeItem(environment.localTokenContentStorageName);
    localStorage.removeItem(environment.kisTokenContentStorageName);
    localStorage.removeItem(environment.kisLoggedInUserInformationStorageName);

    this.localTokenContent = new LocalTokenContent();
    this.kisTokenContent = new KisTokenContent();

    this.user = new User();

    clearInterval(this.refreshLocalTokenIntervalHandle);

    this.router.navigate(['']).then();
  }

  refreshAuthTokenIfExpired() {
    let accessToken = this.getAccessToken();
    if (accessToken) {
      if (this.jwtHelper.isTokenExpired(accessToken)) {
        this.refreshAuthToken()
      }
    }
  }

  isLoggedIn(): boolean {
    return this.getAccessToken() != null;
  }

  isLoggedOut(): boolean {
    return !this.isLoggedIn();
  }

  refreshAuthToken() {
    this.http.get<AccessTokens>(`${AUTH_API}/refreshedAccessToken`).toPromise()
      .then((res) => {
        this.handleAccessTokens(res);
      }).catch((error: any) => {
        this.toastr.error("Obnovení JWT tokenu selhalo.", "Autentizace");
        return throwError(error);
    });
  }

  refreshKisToken() {
    let params = new HttpParams().set('refresh_token', localStorage.getItem(environment.kisRefreshTokenStorageName) ?? this.localTokenContent.krt);
    this.http.get<KisRefreshTokenResponse>(`${environment.kisApiUrl}/auth/fresh_token`, { params }).toPromise()
      .then((res) => {
        localStorage.setItem(environment.kisAccessTokenStorageName, res.auth_token);
        localStorage.setItem(environment.kisRefreshTokenStorageName, res.refresh_token);
        this.decodeKisToken();
        this.getInformationAboutUser();
      }).catch((error: any) => {
        this.toastr.error("Obnovení JWT tokenu selhalo.", "Autentizace");
        return throwError(error);
    });
  }

  private decodeLocalToken() {
    let token = this.getAccessToken();
    if (token != null) {
      this.localTokenContent = this.jwtHelper.decodeToken(token) as LocalTokenContent;
      localStorage.setItem(environment.localTokenContentStorageName, JSON.stringify(this.localTokenContent));
    }
  }

  private decodeKisToken() {
    let token = this.getKisAccessToken();
    if (token != null) {
      this.kisTokenContent = this.jwtHelper.decodeToken(token) as KisTokenContent;
      localStorage.setItem(environment.kisTokenContentStorageName, JSON.stringify(this.kisTokenContent));
    }
  }

  getUserRoles() {
    return this.localTokenContent.role;
  }

  hasRole(role_type: RoleTypes): boolean {
    if (this.isLoggedIn()) {
      if (this.localTokenContent.role) {
        return this.localTokenContent.role.indexOf(role_type) !== -1;
      }
    }
    return false;
  }

  isStatesManager(): boolean {
    return this.hasRole(RoleTypes.StatesManager) || this.hasRole(RoleTypes.Admin);
  }

  isEventsManager(): boolean {
    return this.hasRole(RoleTypes.EventsManager) || this.hasRole(RoleTypes.Admin);
  }

  isBoardGamesManager(): boolean {
    return this.hasRole(RoleTypes.BoardGamesManager) || this.hasRole(RoleTypes.Admin);
  }

  isAdmin(): boolean {
    return this.hasRole(RoleTypes.Admin);
  }

  getInformationAboutUser() {
    this.http.get<KisLoggedInUserInformation>(`${environment.kisApiUrl}/users/me`).toPromise()
      .then((res) => {
        this.kisLoggedInUserInformation = res;
        localStorage.setItem(environment.kisLoggedInUserInformationStorageName, JSON.stringify(this.kisLoggedInUserInformation));

        this.assignDataFromKisUserInformation();
        this.assignDataFromLocalTokenContent();
        this.storeUserDataToStorage();
      }).catch((error: any) => {
        throwError(error);
        this.toastr.error("Stažení informací o uživateli z KIS se nezdařilo.", "Autentizace");
    });
  }

  getUserName() {
    return this.user.name;
  }

  private assignDataFromLocalTokenContent() {
  }

  private assignDataFromKisUserInformation() {
    this.user.nickname = this.kisLoggedInUserInformation.nickname;
    this.user.name = this.kisLoggedInUserInformation.name;
    this.user.email = this.kisLoggedInUserInformation.email;
    this.user.cardCode = this.kisLoggedInUserInformation.pin;
    this.user.gamificationConsent = this.kisLoggedInUserInformation.gamification_consent;
    this.user.prestige = this.kisLoggedInUserInformation.prestige;
  }

  /**
   * Updates user data after refresh of website, whenever the user is logged in.
   *
   * @remarks
   * If the data are stored in storage, takes the data from there,
   * otherwise requests new user data from its respective endpoints.
   */
  updateUserDataIfLoggedIn(): void {
    if (this.isLoggedIn()) {
      let userDataFromStorage = localStorage.getItem(environment.userDataStorageName);
      if (userDataFromStorage != null) {
        this.user = JSON.parse(<string>userDataFromStorage);
      } else {
        this.getInformationAboutUser();
      }

      let localTokenContentFromStorage = localStorage.getItem(environment.localTokenContentStorageName);
      if (localTokenContentFromStorage != null) {
        this.localTokenContent = JSON.parse(localTokenContentFromStorage);
      } else {
        // TODO: Get local token content.
      }

      let kisTokenContentFromStorage = localStorage.getItem(environment.kisTokenContentStorageName);
      if (kisTokenContentFromStorage != null) {
        this.kisTokenContent = JSON.parse(kisTokenContentFromStorage);
      } else {
        // TODO: Get local token content.
      }

      let kisLoggedInUserInformationFromStorage = localStorage.getItem(environment.kisLoggedInUserInformationStorageName);
      if (kisLoggedInUserInformationFromStorage != null) {
        this.kisLoggedInUserInformation = JSON.parse(kisLoggedInUserInformationFromStorage);
      } else {
        this.getInformationAboutUser();
      }
    }
  }

  /**
   * Stores user data to the storage.
   */
  private storeUserDataToStorage() {
    if (this.isLoggedIn()) {
      localStorage.setItem(environment.userDataStorageName, JSON.stringify(this.user));
    }
  }

  getAccessToken() {
    return localStorage.getItem(environment.accessTokenStorageName);
  }

  getKisAccessToken() {
    return localStorage.getItem(environment.kisAccessTokenStorageName);
  }

  getKisRefreshToken() {
    return localStorage.getItem(environment.kisRefreshTokenStorageName);
  }
}
