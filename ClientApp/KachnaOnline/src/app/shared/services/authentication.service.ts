import { environment } from '../../../environments/environment';
import { KisResponse } from '../../models/kis-response.model';
import { ToastrService } from 'ngx-toastr';
import { Router, ActivatedRoute } from '@angular/router';
import { Injectable } from '@angular/core';
import { HttpParams, HttpClient } from '@angular/common/http';
import { Location } from '@angular/common';
import { JwtHelperService } from '@auth0/angular-jwt';
import {LocalTokenContent} from "../../models/local-token-content.model";


const AUTH_API = `${environment.baseApiUrl}/auth`;
const STORAGE_TOKEN_KEY = 'authenticationToken';

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

  getSessionIdFromKisEduId() {
    let params = new HttpParams().set('redirect', `${window.location.origin}/login`)
    this.http.get<KisResponse>('https://su-int.fit.vutbr.cz/kis/api/auth/eduid', { params: params }).toPromise()
      .then(function(res: KisResponse) {
        let kisResponse = res;
        console.log(kisResponse.wayf_url)
        console.log("Navigating");
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
    this.http.get(`${environment.baseApiUrl}/auth/accessTokenFromSession`, { params: params, responseType: 'text'}).subscribe(
      res  => { // TODO: Parse JSON with access tokens information.
        /*res as AccessTokens*/
        localStorage.setItem(STORAGE_TOKEN_KEY, res);
        this.decodeToken(res);

        setInterval( () => {
            console.log("calling refresh token");
            this.http.get(`${AUTH_API}/refreshedAccessToken`, { responseType: 'text' }).subscribe(
              res => {
                localStorage.setItem(STORAGE_TOKEN_KEY, res);
                console.log(localStorage.getItem(STORAGE_TOKEN_KEY));
              },
              err => {
                console.log(err);
                this.toastr.error("Obnovení JWT tokenu selhalo.", "Autentizace");
                return;
              }
            )
          },
          2500 * 1000);
      },
      err => {
        console.log(err);
        this.toastr.error("Načtení přístupových údajů selhalo.", "Autentizace");
        return;
      }
    );

  }

  logOut() {
    localStorage.removeItem(STORAGE_TOKEN_KEY)
  }

  checkForAuthTokenExpirationAndRefreshAuthToken() {
    if (this.jwtHelper.isTokenExpired()) {
      this.refreshAuthToken()
    }
  }

  isLoggedIn(): boolean {
    return localStorage.getItem(STORAGE_TOKEN_KEY) != null;
  }


  isLoggedOut(): boolean {
    return !this.isLoggedIn();
  }

  refreshAuthToken() {
    this.http.get(`${AUTH_API}/refreshedAccessToken`, { responseType: 'text' }).subscribe(
      res => {
        localStorage.setItem(STORAGE_TOKEN_KEY, res);
        this.decodeToken(res);
      },
      err => {
        console.log(err);
        this.toastr.error("Obnovení JWT tokenu selhalo.", "Autentizace");
        return;
      });
  }

  private decodeToken(token: string) {
    this.localTokenContent = this.jwtHelper.decodeToken(token) as LocalTokenContent;
    console.log(this.localTokenContent);
  }

}
