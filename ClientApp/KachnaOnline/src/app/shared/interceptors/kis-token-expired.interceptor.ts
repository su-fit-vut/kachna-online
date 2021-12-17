// kis-token-interceptor.interceptor.ts
// Author: David Chocholatý

import {
  HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpClient, HttpErrorResponse, HttpParams
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from "rxjs/operators";
import { AuthenticationService } from "../services/authentication.service";
import { environment } from "../../../environments/environment";
import { KisRefreshTokenResponse } from "../../models/users/auth/kis/kis-refresh-token-response.model";
import { ToastrService } from "ngx-toastr";
import { Injectable } from "@angular/core";

@Injectable()
export class KisTokenExpiredInterceptor implements HttpInterceptor {
  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService,
    private toastr: ToastrService,
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.url.includes("/kis/api/")) {
      if (request.url.includes("kis/api/auth")) {
        return next.handle(request);
      }

      return next.handle(request).pipe(catchError(err => {
        if (err instanceof HttpErrorResponse) {
          if (err.status === 401) {
            let params = new HttpParams().set('refresh_token', localStorage.getItem(environment.kisRefreshTokenStorageName) ?? this.authenticationService.localTokenContent.krt);
            return this.http.get<KisRefreshTokenResponse>(`${environment.kisApiUrl}/auth/fresh_token`, {params}).pipe(
              switchMap((res) => {
                localStorage.setItem(environment.kisAccessTokenStorageName, res.auth_token);
                localStorage.setItem(environment.kisRefreshTokenStorageName, res.refresh_token);
                this.authenticationService.decodeKisToken();
                this.authenticationService.getInformationAboutUser().then();
                return next.handle(request.clone({
                  setHeaders: {
                    'Authorization': `Bearer ${res.auth_token}`,
                  }
                }));
              }),
              catchError((err) => {
                this.toastr.error("Obnovení přihlášení selhalo. Přihlaš se znovu.", "Přihlášení");
                this.authenticationService.logOut();
                return throwError(err);
              })
            );
          }
        }
        return throwError(err);
      }));
    }
    return next.handle(request);
  }
}
