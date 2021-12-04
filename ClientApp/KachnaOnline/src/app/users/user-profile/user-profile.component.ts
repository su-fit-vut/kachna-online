// user-profile.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from "../../shared/services/authentication.service";
import { NgForm } from "@angular/forms";
import { prestigeText } from "../registration/registration.component";

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

  gamificationConsentTooltipText:string = prestigeText;
  cardCodeTooltipText:string = "Pro přiřazení karty ke svému účtu řekni někomu za barem. Pípneš si kartou a barman ti dá kód, který sem napíšeš."

  ngOnInit(): void {
    this.authenticationService.updateLocalUserInformation();
  }

  ngOnDestroy(): void {
  }

  onSaveChanges(form: NgForm) {
    this.authenticationService.userInfoSaved();
  }
}
