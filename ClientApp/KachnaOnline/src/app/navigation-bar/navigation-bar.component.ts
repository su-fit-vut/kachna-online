// navigation-bar.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from "../shared/services/authentication.service";
import { BoardGamesService } from "../shared/services/board-games.service";
import { ReservationState } from "../models/board-games/reservation.model";
import { Router } from "@angular/router";

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css']
})
export class NavigationBarComponent implements OnInit {

  constructor(
    public authenticationService: AuthenticationService,
    public boardGamesService: BoardGamesService,
    public router: Router
  ) { }

  ngOnInit(): void {
  }

  navigateToCurrentReservations(): void {
    this.isMenuCollapsed = true;
    this.boardGamesService.saveManagerFilter(ReservationState.Current);
    this.router.navigate(['/board-games/manager/reservations']).then();
  }

  navigateToAllReservations(): void {
    this.isMenuCollapsed = true;
    this.boardGamesService.saveManagerFilter(undefined);
    this.router.navigate(['/board-games/manager/reservations']).then();
  }

  public isMenuCollapsed = true;
}
