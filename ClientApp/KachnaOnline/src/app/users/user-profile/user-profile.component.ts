import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from "../../shared/services/authentication.service";
import { FormBuilder, FormControl, NgForm, Validators } from "@angular/forms";
import { prestigeText } from "../registration/registration.component";
import { UserDetail } from "../../models/users/user.model";
import { ToastrService } from "ngx-toastr";
import { KisLoggedInUserInformation } from "../../models/users/kis-logged-in-user-information.model";
import { throwError } from "rxjs";

@Component({
  selector: 'app-user',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {
  public isCardInfoCollapsed: boolean = true;

  constructor(
    public authenticationService: AuthenticationService,
    private fb: FormBuilder,
    private toastrService: ToastrService,
  ) { }

  gamificationConsentTooltipText:string = prestigeText;
  cardCodeTooltipText:string = "Pro přiřazení karty ke svému účtu řekni někomu za barem. Pípneš si kartou a barman ti dá kód, který sem napíšeš."

  form = this.fb.group({
    name: [""],
    email: [""],
    nickname: ["", [Validators.maxLength(128)]],
    gamificationConsent: [false],
    cardCode: ["", [Validators.maxLength(5), Validators.pattern("[a-zA-Z]{5}")]],
  })
  ngOnInit(): void {
    this.authenticationService.getLocalUserInformationRequest().toPromise()
      .then((userDetail: UserDetail) => {
        this.form.controls.nickname.setValue(userDetail.nickname);
      }).catch((err) => {
      console.log(err);
      this.toastrService.error("Nebylo možné načíst data o uživateli.", "Správa účtu");
    });

    this.authenticationService.getInformationAboutLoggedInUserFromKisRequest().toPromise()
      .then((kisLoggedInUserInformation: KisLoggedInUserInformation) => {
        this.form.controls.name.setValue(kisLoggedInUserInformation.name);
        this.form.controls.email.setValue(kisLoggedInUserInformation.email);
        //this.form.controls.nickname.setValue(kisLoggedInUserInformation.nickname);
        this.form.controls.gamificationConsent.setValue(kisLoggedInUserInformation.gamification_consent);
      }).catch(err => {
        this.toastrService.error("Nebylo možné načíst data o uživateli.", "Správa účtu");
        throwError(err);
    });
  }

  ngOnDestroy(): void {
  }

  onSaveChanges() {
    let formVal = this.form.controls;
    if (formVal.nickname.dirty) {
      this.authenticationService.updateNickname(formVal.nickname.value);
    }

    if (formVal.gamificationConsent.dirty) {
      this.authenticationService.changeGamificationConsent(formVal.gamificationConsent.value);
    }

    if (formVal.cardCode.dirty) {
      this.authenticationService.changeCardCode(formVal.cardCode.value);
    }

    this.form.markAsPristine();
  }
}
