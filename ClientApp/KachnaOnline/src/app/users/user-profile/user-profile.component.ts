// user-profile.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from "../../shared/services/authentication.service";
import { NgForm } from "@angular/forms";

@Component({
  selector: 'app-user',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {
  public isCardInfoCollapsed: boolean = true;

  constructor(
    public authenticationService: AuthenticationService,
  ) { }

  gamificationConsentTooltipText:string = "Tímto souhlasíš s gamifikací v klubu U Kachničky."
  cardCodeTooltipText:string = "Tohle je kód tvé klubovní karty."

  ngOnInit(): void {
    this.authenticationService.updateLocalUserInformation();
  }

  ngOnDestroy(): void {
  }

  onSaveChanges(form: NgForm) {
    this.authenticationService.userInfoSaved();
  }
}
