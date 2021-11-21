import { KisResponse } from './../../models/kis-response.model';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { HttpParams, HttpClient, HttpResponse } from '@angular/common/http';
import { waitForAsync } from '@angular/core/testing';
import { Location } from '@angular/common';

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
}
