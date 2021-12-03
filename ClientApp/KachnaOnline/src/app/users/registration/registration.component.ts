import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from "@angular/forms";
import { StateModification } from "../../models/states/state-modification.model";
import { ClubStateTypes } from "../../models/states/club-state-types.model";
import { NgbDateStruct, NgbTimeStruct } from "@ng-bootstrap/ng-bootstrap";
import { AuthenticationService } from "../../shared/services/authentication.service";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {
  constructor(
    private authenticationService: AuthenticationService,
    private toastr: ToastrService,
  ) { }

  form = new FormGroup({
    sympathizingMemberConsent: new FormControl(false),
    gamificationConsent: new FormControl(true),
  });
  gamificationConsentTooltipText: string = "Informace o souhlasu s gamifikací."; // TODO: Add actual tooltip text.


  ngOnInit(): void {
  }

  onSubmit() {
    /*
    let data = new StateModification(); // FIXME: Registration model? Or just simple object?
    const formValues = this.form.value;

    data.consent = formValues.consent;
    data.gamification_consent = formValues.gamification_consent;

    this.authenticationService.register(data).toPromise()
      .then(res => {
        this.toastr.success("Registrace proběhla úspěšně.", "Registrace uživatele");
      }).catch(err => {
      this.toastr.error("Registrace selhala.", "Registrace uživatele");
    }); // TODO
     */
  }
}
