// home.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from "../shared/services/authentication.service";
import { ClubState } from "../models/states/club-state.model";
import { ClubStateTypes } from "../models/states/club-state-types.model";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(public authenticationService: AuthenticationService, public modalService: NgbModal) {
  }

  ST = ClubStateTypes;
  state: ClubState;

  ngOnInit(): void {
    this.state = {
      id: 1,
      state: ClubStateTypes.OpenBar,
      madeByUser: {
        id: 195,
        name: "LeO"
      },
      start: new Date(2021, 11, 28, 10, 11, 1),
      plannedEnd: new Date(2021, 11, 28, 14, 28, 32),
      note: "Velmi pěkná veřejná poznámka",
      noteInternal: null,//"Interní poznámka se zobrazuje jen manažerům",
      actualEnd: null,
      closedByUser: null,
      eventId: null,
      followingState: null
    }
  }

}
