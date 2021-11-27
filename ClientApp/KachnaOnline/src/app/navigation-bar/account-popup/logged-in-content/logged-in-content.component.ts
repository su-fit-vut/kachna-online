// logged-in-content.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from "../../../shared/services/authentication.service";

@Component({
  selector: 'app-logged-in-content',
  templateUrl: './logged-in-content.component.html',
  styleUrls: ['./logged-in-content.component.css']
})
export class LoggedInContentComponent implements OnInit {

  constructor(
    public authenticationService: AuthenticationService,
  ) { }

  ngOnInit(): void {
  }

  clickChangeCard() {

  }

  clickLogOutButton() {
    this.authenticationService.logOut();
  }

}
