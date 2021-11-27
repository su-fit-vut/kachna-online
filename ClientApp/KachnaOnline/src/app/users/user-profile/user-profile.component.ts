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

  gamificationConsentTooltipText:string = "Tohle je souhlas s gamifikací."
  cardCodeTooltipText:string = "Tohle je krásná karta."

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
  }

  onSaveChanges(form: NgForm) {
    // TODO: Save changes to user nickname, card and gamification approval.
  }
}
