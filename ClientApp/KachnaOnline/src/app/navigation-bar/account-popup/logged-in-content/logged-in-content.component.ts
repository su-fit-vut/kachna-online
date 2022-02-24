import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from "../../../shared/services/authentication.service";
import { Router } from "@angular/router";

@Component({
  selector: 'app-logged-in-content',
  templateUrl: './logged-in-content.component.html',
  styleUrls: ['./logged-in-content.component.css']
})
export class LoggedInContentComponent implements OnInit {

  constructor(
    public authenticationService: AuthenticationService,
    public router: Router
  ) { }

  ngOnInit(): void {
  }

  clickLogOutButton() {
    this.authenticationService.logOut();
    window.location.reload();
  }

}
