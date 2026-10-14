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
  public isLoadingWalletLink: boolean = false;

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

  onAddToGoogleWallet(): void {
    this.isLoadingWalletLink = true;
    this.authenticationService.getGoogleWalletSaveLink()
      .then((res: { url: string }) => {
        this.isLoadingWalletLink = false;
        if (res && res.url) {
          const opened = window.open(res.url, '_blank');
          if (!opened || opened.closed || typeof opened.closed === 'undefined') {
            window.location.href = res.url;
          }
        } else {
          this.toastrService.error("Nebylo možné získat odkaz pro Peněženku Google.", "Digitální karta");
        }
      })
      .catch((err: any) => {
        this.isLoadingWalletLink = false;
        console.error("Failed to fetch Google Wallet save link:", err);
        if (err?.status === 0) {
          this.toastrService.error("Nepodařilo se připojit k serveru. Zkontrolujte připojení.", "Digitální karta");
        } else if (err?.status === 401 || err?.status === 403) {
          this.toastrService.error("Pro přidání karty do Peněženky Google se prosím znovu přihlaste.", "Digitální karta");
        } else {
          this.toastrService.error("Nebylo možné vygenerovat odkaz pro Peněženku Google.", "Digitální karta");
        }
      });
  }
}
