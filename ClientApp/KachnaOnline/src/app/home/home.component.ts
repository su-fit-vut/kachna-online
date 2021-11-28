// home.component.ts
// Author: David Chocholatý

import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthenticationService } from "../shared/services/authentication.service";
import { ClubState } from "../models/states/club-state.model";
import { ClubStateTypes } from "../models/states/club-state-types.model";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { StatesService } from "../shared/services/states.service";
import { Subscription, timer } from "rxjs";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  constructor(public authenticationService: AuthenticationService,
              public stateService: StatesService,
              public modalService: NgbModal) {
  }

  ST = ClubStateTypes;

  state: ClubState;
  reloadSubscription: Subscription;

  ngOnInit(): void {
    this.reloadSubscription = timer(0, 60000)
      .subscribe(_ => { this.stateService.getCurrent().subscribe(result => this.state = result);});
  }

  ngOnDestroy(): void {
    this.reloadSubscription.unsubscribe()
  }

}
