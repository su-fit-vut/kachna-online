// login.component.ts
// Author: David Chocholatý

import { AuthenticationService } from '../../shared/services/authentication.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { ToastrService } from 'ngx-toastr';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, ComponentFactoryResolver, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from "../../../environments/environment";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private toastr: ToastrService,
    private jwtHelper: JwtHelperService,
    private authenticationService: AuthenticationService,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.authenticationService.logIn();

    let returnAddress = localStorage.getItem(environment.returnAddressStorageName);
    this.router.navigate( (returnAddress != null) ? [returnAddress] : ['.']).then();
  }
}
