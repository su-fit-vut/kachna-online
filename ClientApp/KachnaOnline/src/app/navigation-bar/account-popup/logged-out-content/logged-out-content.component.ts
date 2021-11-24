import { Component, OnInit } from '@angular/core';
import {AuthenticationService} from "../../../shared/services/authentication.service";

@Component({
  selector: 'app-logged-out-content',
  templateUrl: './logged-out-content.component.html',
  styleUrls: ['./logged-out-content.component.css']
})
export class LoggedOutContentComponent implements OnInit {

  constructor(
    public authenticationService: AuthenticationService,
  ) { }

  ngOnInit(): void {
  }

  clickLogInButton() {
    this.authenticationService.getSessionIdFromKisEduId();
  }

  clickRegisterButton() {

  }

}
