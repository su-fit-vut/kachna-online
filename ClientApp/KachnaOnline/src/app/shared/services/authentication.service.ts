// authentication.service.ts
// Author: David Chocholatý

import {environment} from '../../../environments/environment';
import {KisEduIdResponse} from '../../models/kis-eduid-response.model';
import {ToastrService} from 'ngx-toastr';
import {ActivatedRoute, Router} from '@angular/router';
import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Location} from '@angular/common';
import {JwtHelperService} from '@auth0/angular-jwt';
import {LocalTokenContent} from "../../models/local-token-content.model";
import {RoleTypes} from "../../models/role-types.model";
import {User} from "../../models/user.model";
import {AccessTokens} from "../../models/access-tokens.model";
import {KisTokenContent} from "../../models/kis-token-content.model";
import {KisLoggedInUserInformation} from "../../models/kis-logged-in-user-information.model";

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

  user: User = new User();

  getSessionIdFromKisEduId() {
    let params = new HttpParams().set('redirect', `${window.location.origin}/login`)
    this.http.get<KisEduIdResponse>(`${environment.kisApiUrl}/auth/eduid`, { params: params }).toPromise()
      .then(function(res: KisEduIdResponse) {
        let kisResponse = res;
        console.log(kisResponse.wayf_url)
        window.open(kisResponse.wayf_url, '_self');
      }).catch((error: any) => {
        console.log(error);
        this.toastr.error("Přihlášení se ke KIS selhalo.", "Autentizace");
        return;
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

          setInterval( () => {
              this.refreshAuthToken();
            },
            2500 * 1000);
        }).catch((error: any) => {
          console.log(error);
          this.toastr.error("Načtení přístupových údajů selhalo.", "Autentizace");
          return;
    });
  }

  private handleAccessTokens(res: AccessTokens) {
    localStorage.setItem('accessToken', res.accessToken);
    localStorage.setItem('kisAccessToken', res.kisAccessToken);
    this.decodeLocalToken(res.accessToken);
    this.decodeKisToken(res.kisAccessToken);
    this.getInformationAboutUser();
  }

  logOut() {
    localStorage.removeItem(environment.accessTokenStorageName);
    localStorage.removeItem(environment.kisAccessTokenStorageName);
    this.localTokenContent = new LocalTokenContent();
    this.kisTokenContent = new KisTokenContent();

    this.user = new User();

    this.router.navigate(['']).then();
  }

  checkForAuthTokenExpirationAndRefreshAuthToken() {
    if (this.jwtHelper.isTokenExpired()) {
      this.refreshAuthToken()
    }
  }

  isLoggedIn(): boolean {
    return localStorage.getItem(environment.accessTokenStorageName) != null;
  }

  isLoggedOut(): boolean {
    return !this.isLoggedIn();
  }

  refreshAuthToken() {
    this.http.get<AccessTokens>(`${AUTH_API}/refreshedAccessToken`).toPromise()
      .then((res) => {
        this.handleAccessTokens(res);
      }).catch((error: any) => {
      console.log(error);
      this.toastr.error("Obnovení JWT tokenu selhalo.", "Autentizace");
      return;
    });
  }

  private decodeLocalToken(token: string) {
    this.localTokenContent = this.jwtHelper.decodeToken(token) as LocalTokenContent;
  }

  private decodeKisToken(token: string) {
    this.kisTokenContent = this.jwtHelper.decodeToken(token) as KisTokenContent;
  }

  getUserRoles() {
    return this.localTokenContent.role;
  }

  hasRole(role_type: RoleTypes): boolean {
    return this.localTokenContent.role.indexOf(role_type) !== -1;
  }


  getInformationAboutUser() {
    this.http.get<KisLoggedInUserInformation>(`${environment.kisApiUrl}/users/me`).toPromise()
      .then((res) => {
        this.kisLoggedInUserInformation = res;
        console.log(res);
        this.assignDataFromKisUserInformation();
      }).catch((error: any) => {
        console.log(error);
        this.toastr.error("Stažení informací o uživateli z KIS se nezdařilo.", "Autentizace");
    });

    this.assignDataFromLocalTokenContent();
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
    this.user.cardNumber = this.kisLoggedInUserInformation.pin;
    this.user.gamificationConsent = this.kisLoggedInUserInformation.gamification_consent;
    this.user.prestige = this.kisLoggedInUserInformation.prestige;
  }
}
