import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthenticationService } from "../shared/services/authentication.service";
import { ClubState } from "../models/states/club-state.model";
import { ClubStateTypes } from "../models/states/club-state-types.model";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { StatesService } from "../shared/services/states.service";
import { Subscription, timer } from "rxjs";
import { NavigationEnd, Router } from "@angular/router";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  constructor(public authenticationService: AuthenticationService,
              public stateService: StatesService,
              public modalService: NgbModal,
              private router: Router) {
  }

  ST = ClubStateTypes;

  state: ClubState;
  statesReloadSubscription: Subscription;
  routerReloadSubscription: Subscription;

  ngOnInit(): void {
    this.statesReloadSubscription = timer(0, 60000)
      .subscribe(_ => this.loadState());

    this.routerReloadSubscription = this.router.events.subscribe((e: any) => {
      if (e instanceof NavigationEnd) {
        this.loadState();
      }
    });
  }

  ngOnDestroy(): void {
    this.statesReloadSubscription?.unsubscribe()
    this.routerReloadSubscription?.unsubscribe();
  }

  loadState(): void {
    this.stateService.getCurrent().subscribe(result => this.state = result);
  }

}
