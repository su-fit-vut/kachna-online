import { environment } from './../../../environments/environment';
import { KisResponse } from './../../models/kis-response.model';
import { ToastrService } from 'ngx-toastr';
import { Router, ActivatedRoute } from '@angular/router';
import { Injectable } from '@angular/core';
import { HttpParams, HttpClient, HttpResponse, HttpHeaders } from '@angular/common/http';
import { Location } from '@angular/common';
import { JwtHelperService } from '@auth0/angular-jwt';
import { UserService } from './user.service';


const AUTH_API = `${environment.baseApiUrl}/auth`;

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  kisResponse: KisResponse = new KisResponse();


  constructor(
    private router: Router,
    private http: HttpClient,
    private toastr: ToastrService,
    private location: Location,
    public jwtHelper: JwtHelperService,
    private route: ActivatedRoute,
  ) { }

  getSessionIdFromKisEduId() {
    let params = new HttpParams().set('redirect', `${window.location.origin}/login`)
    this.http.get<KisResponse>('https://su-int.fit.vutbr.cz/kis/api/auth/eduid', { params: params }).subscribe(
    res => this.kisResponse = res,
    err => {
      console.log(err);
      this.toastr.error("Přihlášení se ke KIS selhalo.", "Autentizace");
      return;
    }
    );

    window.open(this.kisResponse.wayf_url, '_self');
  }

  logIn() {
    let sessionId = "";
    this.route.queryParams
    .subscribe(params => {
      sessionId = params['session'];
    });

    let params = new HttpParams().set('session', sessionId);

    this.http.get(`${environment.baseApiUrl}/auth/accessTokenFromSession`, { params: params, responseType: 'text'}).subscribe(
      res => {
        localStorage.setItem('authenticationToken', res);
        console.log(this.jwtHelper.tokenGetter())
        setInterval( () => {
            console.log("calling refresh token");
            this.http.get(`${AUTH_API}/refreshedAccessToken`, { responseType: 'text' }).subscribe(
              res => localStorage.setItem('authenticationToken', res),
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

  }

  checkForAuthTokenExpirationAndRefreshAuthToken() {
    if (this.jwtHelper.isTokenExpired()) {
      this.refreshAuthToken()
    }
  }

  refreshAuthToken() {
    console.log("calling refresh token");
    this.http.get(`${AUTH_API}/refreshedAccessToken`, { responseType: 'text' }).subscribe(
      res => localStorage.setItem('authenticationToken', res),
      err => {
        console.log(err);
        this.toastr.error("Obnovení JWT tokenu selhalo.", "Autentizace");
        return;
      });
  }
}
