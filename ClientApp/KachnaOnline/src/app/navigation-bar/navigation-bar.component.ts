// navigation-bar.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from "../shared/services/authentication.service";
import { BoardGamesService } from "../shared/services/board-games.service";
import { ReservationState } from "../models/board-games/reservation.model";
import { Router } from "@angular/router";
import { StatesService } from "../shared/services/states.service";
import { BoardGamesStoreService } from "../shared/services/board-games-store.service";

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css']
})
export class NavigationBarComponent implements OnInit {

  constructor(
    public authenticationService: AuthenticationService,
    public boardGamesService: BoardGamesService,
    public stateService: StatesService,
    public router: Router,
    public storeService: BoardGamesStoreService
  ) {}

  public isMenuCollapsed = true;

  ngOnInit(): void {
  }

  navigateToCurrentReservations(): void {
    this.isMenuCollapsed = true;
    this.storeService.saveManagerFilter(ReservationState.Current);
    this.router.navigate(['/board-games/manager/reservations']).then();
  }

  navigateToAllReservations(): void {
    this.isMenuCollapsed = true;
    this.storeService.saveManagerFilter(undefined);
    this.router.navigate(['/board-games/manager/reservations']).then();
  }

  closeCurrentState(): void {
    this.stateService.closeCurrent().subscribe(_ => this.router.navigate(['/']).finally());
  }
}
