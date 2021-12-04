import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from "@angular/forms";
import { AuthenticationService } from "../../shared/services/authentication.service";
import { ToastrService } from "ngx-toastr";
import { ActivatedRoute, Params } from "@angular/router";

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
  ) { }

  form = new FormGroup({
    sympathizingMemberConsent: new FormControl(false),
    gamificationConsent: new FormControl(true),
  });
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
