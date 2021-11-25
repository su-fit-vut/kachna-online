// home.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from "../shared/services/authentication.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(
    public authenticationService: AuthenticationService,
  ) { }

  jumbotronStateMainText: string = "Kachna je zavřená.";

  ngOnInit(): void {
  }

}
