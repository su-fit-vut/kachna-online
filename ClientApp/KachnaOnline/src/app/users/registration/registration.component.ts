import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from "@angular/forms";
import { StateModification } from "../../models/states/state-modification.model";
import { ClubStateTypes } from "../../models/states/club-state-types.model";
import { NgbDateStruct, NgbTimeStruct } from "@ng-bootstrap/ng-bootstrap";
import { AuthenticationService } from "../../shared/services/authentication.service";
import { ToastrService } from "ngx-toastr";
import { environment } from "../../../environments/environment";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { KisLoggedInUserInformation } from "../../models/users/kis-logged-in-user-information.model";

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {
  constructor(
    private authenticationService: AuthenticationService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
  ) { }

  form = new FormGroup({
    sympathizingMemberConsent: new FormControl(false),
    gamificationConsent: new FormControl(true),
  });
  gamificationConsentTooltipText: string = "Informace o souhlasu s gamifikací."; // TODO: Add actual tooltip text.
  sessionId: string;

  ngOnInit(): void {
    this.route.queryParams.subscribe((params: Params) => {
      this.sessionId = params.session;
    });
  }

  onSubmit() {
    this.authenticationService.register(this.sessionId).then();
  }
}
