import { environment } from '../../../environments/environment';
import { KisEduIdResponse } from '../../models/users/auth/kis/kis-eduid-response.model';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Location } from '@angular/common';
import { JwtHelperService } from '@auth0/angular-jwt';
import { LocalTokenContent } from "../../models/users/auth/local-token-content.model";
import { RoleTypes } from "../../models/users/auth/role-types.model";
import { User, UserDetail } from "../../models/users/user.model";
import { AccessTokens } from "../../models/users/auth/access-tokens.model";
import { KisTokenContent } from "../../models/users/auth/kis/kis-token-content.model";
import { KisLoggedInUserInformation } from "../../models/users/kis-logged-in-user-information.model";
import { KisRefreshTokenResponse } from "../../models/users/auth/kis/kis-refresh-token-response.model";
import { Observable, of, throwError } from "rxjs";

const AUTH_API = `${environment.baseApiUrl}/auth`;
const USERS_API = `${environment.baseApiUrl}/users`;

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  constructor(
    private router: Router,
    private http: HttpClient,
    private toastrService: ToastrService,
    private location: Location,
    public jwtHelper: JwtHelperService,
    private route: ActivatedRoute
  ) {
  }

  localTokenContent: LocalTokenContent = new LocalTokenContent();
  kisTokenContent: KisTokenContent = new KisTokenContent();
  kisLoggedInUserInformation: KisLoggedInUserInformation = new KisLoggedInUserInformation();
  refreshLocalTokenIntervalHandle: number;

  user: User = new User();
  usersList: UserDetail[] = [];
  shownUsersList: UserDetail[] = [];
  public userDetail: UserDetail = new UserDetail();

  getSessionIdFromKisEduId(redirectTo: string = "login") {
    let targetLocation = this.location.prepareExternalUrl(redirectTo);
    let params = new HttpParams().set('redirect', `${window.location.origin}${targetLocation}`)

    this.http.get<KisEduIdResponse>(`${environment.kisApiUrl}/auth/eduid`, {params: params}).toPromise()
      .then((res: KisEduIdResponse) => {
        let kisResponse = res;
        localStorage.setItem(environment.returnAddressStorageName, this.router.url);
        window.open(kisResponse.wayf_url, '_self');
      }).catch((error: any) => {
      this.toastrService.error("Nepoda??ilo se za????dat KIS o p??ihl????en??, zkuste to znovu nebo n??m dejte v??d??t. (Chyba: kis-eduid.)",
        "P??ihl????en??");
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
        this.toastrService.error("P??ihl????en?? selhalo, zkuste to znovu nebo n??m dejte v??d??t. (Chyba: login-session.)", "P??ihl????en??");
        return throwError(error);
      });
  }

  logInRt(refreshToken: string): Promise<any> {
    let params = new HttpParams().set('kisRefreshToken', refreshToken);
    return this.http.get<AccessTokens>(`${AUTH_API}/accessTokenFromRefreshToken`, {params: params}).toPromise()
      .then((res) => {
        return this.handleAccessTokens(res);
      }).catch((error: any) => {
        this.toastrService.error("P??ihl????en?? selhalo, zkuste to znovu nebo n??m dejte v??d??t. (Chyba: login-rt.)", "P??ihl????en??");
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
        this.handleAccessTokens(res).then();
      }).catch((error: any) => {
      if (error.status === 0) {
        this.toastrService.error("Obnoven?? p??ihl????en?? selhalo, zkuste se p??ihl??sit znovu.", "P??ihl????en??");
      } else {
        this.toastrService.info("P??ihl????en?? vypr??elo, p??ihla??te se znovu.", "P??ihl????en??");
      }
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
        this.getInformationAboutUser().then();
      }).catch((error: any) => {
      if (error.status === 0) {
        this.toastrService.error("Obnoven?? p??ihl????en?? selhalo, zkuste se p??ihl??sit znovu.", "P??ihl????en??");
      } else {
        this.toastrService.info("P??ihl????en?? vypr??elo, p??ihla??te se znovu.", "P??ihl????en??");
      }
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

  decodeKisToken() {
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
    return this.hasRole(RoleTypes.StatesManager);
  }

  isEventsManager(): boolean {
    return this.hasRole(RoleTypes.EventsManager);
  }

  isBoardGamesManager(): boolean {
    return this.hasRole(RoleTypes.BoardGamesManager);
  }

  isAdmin(): boolean {
    return this.hasRole(RoleTypes.Admin);
  }

  getInformationAboutUser(): Promise<any> {
    return this.getInformationAboutLoggedInUserFromKisRequest().toPromise()
      .then((res) => {
        this.kisLoggedInUserInformation = res;
        localStorage.setItem(environment.kisLoggedInUserInformationStorageName, JSON.stringify(this.kisLoggedInUserInformation));

        this.assignDataFromKisUserInformation();
        this.assignDataFromLocalTokenContent();
        this.storeUserDataToStorage();
      }).catch((error: any) => {
        throwError(error);
        this.toastrService.error("Sta??en?? informac?? o u??ivateli z KIS se nezda??ilo.", "P??ihl????en??");
      });
  }

  getInformationAboutLoggedInUserFromKisRequest() {
    return this.http.get<KisLoggedInUserInformation>(`${environment.kisApiUrl}/users/me`);
  }

  getUserName() {
    return this.user.name;
  }

  getUserId(): number {
    return this.user.id;
  }

  assignDataFromLocalTokenContent() {
  }

  assignDataFromKisUserInformation() {
    this.user.nickname = (this.user.nickname) ? this.user.nickname : this.kisLoggedInUserInformation.nickname;
    this.user.name = this.kisLoggedInUserInformation.name;
    this.user.email = this.kisLoggedInUserInformation.email;
    this.user.cardCode = "";
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
        this.getInformationAboutUser().then();
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
        this.getInformationAboutUser().then();
      }
    }
  }

  /**
   * Stores user data to the storage.
   */
  storeUserDataToStorage() {
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
      this.toastrService.error("Sta??en?? seznamu u??ivatel?? se nezda??ilo.", "Spr??va u??ivatel??");
    });
  }

  getUsersRequest() {
    return this.http.get<UserDetail[]>(`${environment.baseApiUrl}/users`);
  }

  getFilteredUsers(filter: string): Observable<UserDetail[]> {
    let params = new HttpParams().set('filter', filter);
    return this.http.get<UserDetail[]>(`${environment.baseApiUrl}/users`, {params: params});
  }

  updateNickname(nickname: string) {
    this.updateKisNicknameRequest(nickname).toPromise()
      .then(_ => {
        this.updateLocalNicknameRequest(nickname);
        this.updateLocalUserInformation();
        return;
      }).catch((err: any) => {
      this.handleUpdateNicknameErrors(err.error);
      return;
    });
  }

  updateKisNicknameRequest(nickname: string): Observable<KisLoggedInUserInformation> {
    return this.http.put<KisLoggedInUserInformation>(`${environment.kisApiUrl}/users/me/nickname`, JSON.stringify(nickname),
      {headers: new HttpHeaders({'Content-Type': 'application/json'})});
  }

  updateLocalNicknameRequest(nickname: string) {
    return this.http.put(`${USERS_API}/me/nickname`, JSON.stringify(nickname),
      {headers: new HttpHeaders({'Content-Type': 'application/json'})});
  }

  private handleUpdateNicknameErrors(err: HttpErrorResponse) {
    if (err.status === 0) {
      this.toastrService.error("Nepoda??ilo se odeslat po??adavek na zm??nu p??ezd??vky. Funguje v??m internet? Zkuste to znovu nebo n??m dejte v??d??t. (Chyba: kis-nick.)", "Zm??na u??ivatelsk??ch ??daj??");
    } else if (err.status === 409) {
      this.toastrService.error("Po??adovanou p??ezd??vku u?? n??kdo pou????v??.", "Zm??na u??ivatelsk??ch ??daj??");
    } else if (err.status === 429) {
      this.toastrService.error("P??ezd??vka m????e b??t zm??n??na jen jednou za 3 m??s??ce.", "Zm??na u??ivatelsk??ch ??daj??");
    }
  }

  updateLocalUserInformation() {
    this.getLocalUserInformationRequest().toPromise()
      .then((userDetail: UserDetail) => {
        this.user.nickname = userDetail.nickname;
      }).catch((err) => {
      console.log(err);
      this.toastrService.error("Nastala neo??ek??van?? chyba. Zkuste se pros??m odhl??sit a znovu p??ihl??sit.", "Spr??va ????tu");
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

  register(sessionId: string): Promise<any> {
    let params = new HttpParams().set('session', sessionId);
    return this.http.post<KisRefreshTokenResponse>(`${environment.kisApiUrl}/auth/eduid/register`, "", {params: params}).toPromise()
      .then((res) => {
        this.logInRt(res.refresh_token).then(() => {
          this.toastrService.success("Registrace prob??hla ??sp????n??.", "Registrace u??ivatele");
          this.router.navigate(['user-profile']).then();
          return;
        });
      }).catch((error) => {
        if (error.error.detail == "User is already registered") {
          this.logIn().then(_ => {
            this.router.navigate(['user-profile']).then();
            return;
          }).catch(_ => {
            this.toastrService.error("Nastala neo??ek??van?? chyba. Zkuste cel??m procesem proj??t znovu.",
              "Registrace u??ivatele");
          })
        } else {
          this.toastrService.error("Registrace selhala.", "Registrace u??ivatele");
          return throwError(error);
        }
        return;
      });
  }

  changeGamificationConsent(gamificationConsent: boolean) {
    this.http.put<KisLoggedInUserInformation>(`${environment.kisApiUrl}/users/me/gamification_consent`,
      gamificationConsent).toPromise()
      .then((res: KisLoggedInUserInformation) => {
        this.kisLoggedInUserInformation = res;
        localStorage.setItem(environment.kisLoggedInUserInformationStorageName, JSON.stringify(this.kisLoggedInUserInformation));

        this.assignDataFromKisUserInformation();
        this.assignDataFromLocalTokenContent();
        this.storeUserDataToStorage();
        this.toastrService.success("Zm??na souhlasu s gamifikac?? prob??hla ??sp????n??.", "Zm??na u??ivatelsk??ch ??daj??");
      }).catch((error: any) => {
      throwError(error);
      this.toastrService.error(`Zm??na souhlasu s gamifikac?? selhala. Zkuste to znovu nebo n??m dejte v??d??t. (Chyba: kis-consent/${error.status}.)`, "Zm??na u??ivatelsk??ch ??daj??");
    });
  }

  changeCardCode(cardCode: string) {
    this.changeCardCodeRequest(cardCode).toPromise()
      .then(_ => {
        this.toastrService.success("P??i??azen?? karty prob??hlo ??sp????n??.", "Zm??na u??ivatelsk??ch ??daj??");
      }).catch(err => {
      this.handleChangeCardCodeErrors(err.error)
    });
  }

  changeCardCodeRequest(cardCode: string) {
    return this.http.put<KisLoggedInUserInformation>(`${environment.kisApiUrl}/users/me/rfid`,
      JSON.stringify(cardCode.toUpperCase()), {headers: new HttpHeaders({'Content-Type': 'application/json'})});
  }

  private handleChangeCardCodeErrors(err: HttpErrorResponse) {
    if (err.status === 0) {
      this.toastrService.error("Nepoda??ilo se odeslat po??adavek na nastaven?? karty. Funguje v??m internet? Zkuste to znovu nebo n??m dejte v??d??t. (Chyba: kis-card.)", "Zm??na u??ivatelsk??ch ??daj??");
    } else if (err.status === 404) {
      this.toastrService.error("Zadan?? p??i??azovac?? k??d neexistuje, vy????dejte si u baru nov??.", "Zm??na u??ivatelsk??ch ??daj??");
    } else if (err.status === 409) {
      this.toastrService.error("Va??e karta nem????e b??t pou??ita (v syst??mu u?? existuje karta se stejn??m ID).", "Zm??na u??ivatelsk??ch ??daj??");
    } else if (err.status === 429) {
      this.toastrService.error("Karta m????e b??t zm??n??na pouze jednou za 6 m??s??c??.", "Zm??na u??ivatelsk??ch ??daj??");
    }
  }
}
