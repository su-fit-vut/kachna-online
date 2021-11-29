// authentication.service.ts
// Author: David Chocholatý

import { environment } from '../../../environments/environment';
import { KisEduIdResponse } from '../../models/users/auth/kis/kis-eduid-response.model';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HTTP_INTERCEPTORS, HttpInterceptor, HttpHeaders } from '@angular/common/http';
import { Location } from '@angular/common';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LocalTokenContent } from "../../models/users/auth/local-token-content.model";
import { RoleTypes } from "../../models/users/auth/role-types.model";
import { User, UserDetail } from "../../models/users/user.model";
import { AccessTokens } from "../../models/users/auth/access-tokens.model";
import { KisTokenContent } from "../../models/users/auth/kis/kis-token-content.model";
import { KisLoggedInUserInformation } from "../../models/users/kis-logged-in-user-information.model";
import { KisRefreshTokenResponse } from "../../models/users/auth/kis/kis-refresh-token-response.model";
import { throwError } from "rxjs";
import { Event } from "../../models/events/event.model";

const AUTH_API = `${environment.baseApiUrl}/auth`;
const USERS_API = `${environment.baseApiUrl}/users`;

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
    private route: ActivatedRoute
  ) { }

  localTokenContent: LocalTokenContent = new LocalTokenContent();
  kisTokenContent: KisTokenContent = new KisTokenContent();
  kisLoggedInUserInformation: KisLoggedInUserInformation = new KisLoggedInUserInformation();
  refreshLocalTokenIntervalHandle: number;

  user: User = new User();
  usersList: UserDetail[] = [];
  shownUsersList: UserDetail[] = [];
  public userDetail: UserDetail = new UserDetail();

  getSessionIdFromKisEduId() {
    let targetLocation = this.location.prepareExternalUrl("/login");
    let params = new HttpParams().set('redirect', `${window.location.origin}${targetLocation}`)

    this.http.get<KisEduIdResponse>(`${environment.kisApiUrl}/auth/eduid`, {params: params}).toPromise()
      .then((res: KisEduIdResponse) => {
        let kisResponse = res;
        localStorage.setItem(environment.returnAddressStorageName, this.router.url);
        window.open(kisResponse.wayf_url, '_self');
      }).catch((error: any) => {
      this.toastr.error("Přihlášení do KIS selhalo.", "Přihlášení");
      return throwError(error);
    });
  }

  logIn(): Promise<any> {
    let sessionId: string = "";
    this.route.queryParams
      .subscribe(params => {
        sessionId = params['session'];
      });

    let params = new HttpParams().set('session', sessionId);
    return this.http.get<AccessTokens>(`${AUTH_API}/accessTokenFromSession`, {params: params}).toPromise()
      .then((res) => {
        return this.handleAccessTokens(res);
      }).catch((error: any) => {
        this.toastr.error("Načtení přístupových údajů selhalo.", "Přihlášení");
        return throwError(error);
      });
  }

  private setRefreshIntervalForAuthToken() {
    clearInterval(this.refreshLocalTokenIntervalHandle);
    this.refreshLocalTokenIntervalHandle = setInterval(() => {
        this.refreshAuthToken();
      },
      this.countLocalAccessTokenRefreshIntervalTime());
  }

  private countLocalAccessTokenRefreshIntervalTime() {
    return Math.floor((this.localTokenContent.exp * 1000 - Date.now()) * 0.8);
  }

  private handleAccessTokens(res: AccessTokens): Promise<any> {
    localStorage.setItem(environment.accessTokenStorageName, res.accessToken);
    localStorage.setItem(environment.kisAccessTokenStorageName, res.kisAccessToken);

    this.decodeLocalToken();
    this.decodeKisToken();
    this.setRefreshIntervalForAuthToken();
    return this.getInformationAboutUser();
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

  refreshAuthTokenIfLoggedIn() {
    if (this.isLoggedIn()) {
      this.refreshAuthToken();
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
      this.toastr.error("Obnovení přihlášení selhalo. Přihlaš se znovu.", "Přihlášení");
      this.logOut();
      return throwError(error);
    });
  }

  refreshKisToken() {
    let params = new HttpParams().set('refresh_token', localStorage.getItem(environment.kisRefreshTokenStorageName) ?? this.localTokenContent.krt);
    this.http.get<KisRefreshTokenResponse>(`${environment.kisApiUrl}/auth/fresh_token`, {params}).toPromise()
      .then((res) => {
        localStorage.setItem(environment.kisAccessTokenStorageName, res.auth_token);
        localStorage.setItem(environment.kisRefreshTokenStorageName, res.refresh_token);
        this.decodeKisToken();
        this.getInformationAboutUser();
      }).catch((error: any) => {
      this.toastr.error("Obnovení přihlášení selhalo. Přihlaš se znovu.", "Přihlášení");
      this.logOut();
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

  getInformationAboutUser(): Promise<any> {
    return this.http.get<KisLoggedInUserInformation>(`${environment.kisApiUrl}/users/me`).toPromise()
      .then((res) => {
        this.kisLoggedInUserInformation = res;
        localStorage.setItem(environment.kisLoggedInUserInformationStorageName, JSON.stringify(this.kisLoggedInUserInformation));

        this.assignDataFromKisUserInformation();
        this.assignDataFromLocalTokenContent();
        this.storeUserDataToStorage();
      }).catch((error: any) => {
        throwError(error);
        this.toastr.error("Stažení informací o uživateli z KIS se nezdařilo.", "Přihlášení");
      });
  }

  getUserName() {
    return this.user.name;
  }

  private assignDataFromLocalTokenContent() {
  }

  private assignDataFromKisUserInformation() {
    this.user.nickname = (this.user.nickname) ? this.user.nickname : this.kisLoggedInUserInformation.nickname;
    this.user.name = this.kisLoggedInUserInformation.name;
    this.user.email = this.kisLoggedInUserInformation.email;
    this.user.cardCode = this.kisLoggedInUserInformation.pin;
    this.user.gamificationConsent = this.kisLoggedInUserInformation.gamification_consent;
    this.user.prestige = this.kisLoggedInUserInformation.prestige;
    this.user.id = this.kisLoggedInUserInformation.id;
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
        this.refreshAuthToken();
      }

      let kisTokenContentFromStorage = localStorage.getItem(environment.kisTokenContentStorageName);
      if (kisTokenContentFromStorage != null) {
        this.kisTokenContent = JSON.parse(kisTokenContentFromStorage);
      } else {
        this.refreshKisToken();
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

  initializeUserDataIfLoggedIn() {
    this.refreshAuthTokenIfLoggedIn();
    this.updateUserDataIfLoggedIn();
  }

  refreshUsersList() {
    this.getUsersRequest().toPromise()
      .then(users => {
        this.usersList = users;
        this.shownUsersList = this.usersList;
      }).catch((error: any) => {
      throwError(error);
      this.toastr.error("Stažení seznamu uživatelů se nezdařilo.", "Správa uživatelů");
    });
  }

  getUsersRequest() {
    return this.http.get<UserDetail[]>(`${environment.baseApiUrl}/users`);
  }

  userInfoSaved() {
    this.updateNicknameRequest().toPromise()
      .then(_ => {
        this.toastr.success("Přezdívka úspěšně aktualizována.", "Správa účtu");
        this.updateLocalUserInformation();
      }).catch((err) => {
      console.log(err);
      this.toastr.error("Přezdívku nebylo možné aktualizovat.", "Správa účtu");
    });
  }

  updateNicknameRequest() {
    return this.http.put<any>(`${USERS_API}/me/nickname`, JSON.stringify(this.user.nickname),
      {headers: new HttpHeaders({'Content-Type': 'application/json'})});
  }

  updateLocalUserInformation() {
    this.getLocalUserInformationRequest().toPromise()
      .then((userDetail: UserDetail) => {
        this.user.nickname = userDetail.nickname;
      }).catch((err) => {
      console.log(err);
      this.toastr.error("Nebylo možné aktualizovat data o uživateli aktualizovat.", "Správa účtu");
    });
  }

  getLocalUserInformationRequest() {
    return this.http.get<UserDetail>(`${USERS_API}/me/`);
  }

  getUserDetailRequest(userId: number) {
    return this.http.get<UserDetail>(`${USERS_API}/${userId}`);
  }

  removeUserRoleRequest(userId: number, userRole: string) {
    let params = new HttpParams().set("state", false);
    return this.http.put(`${USERS_API}/${userId}/roles/${userRole}/assignment`, "", {params: params});
  }

  addUserRoleRequest(userId: number, userRole: string) {
    let params = new HttpParams().set("state", true);
    return this.http.put(`${USERS_API}/${userId}/roles/${userRole}/assignment`, "", {params: params});
  }

  resetUserRoleRequest(userId: number, userRole: string) {
    return this.http.delete(`${USERS_API}/${userId}/roles/${userRole}/assignment`);
  }
}
