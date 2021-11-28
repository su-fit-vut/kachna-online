// reservation-details-item-normal.component.ts
// Author: František Nečas

import { Component, EventEmitter, HostListener, Output } from '@angular/core';
import { ReservationDetailsItemComponent } from "../reservation-details-item.component";
import { BoardGamesService } from "../../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { ReservationItem } from "../../../../models/board-games/reservation-item.model";

@Component({
  selector: '[app-reservation-details-item-normal]',
  templateUrl: './reservation-details-item-normal.component.html',
  styleUrls: ['./reservation-details-item-normal.component.css']
})
export class ReservationDetailsItemNormalComponent extends ReservationDetailsItemComponent {
  @HostListener("click") onclick() {
    this.reservationItemClicked.emit(this.item);
  }

  constructor(boardGamesService: BoardGamesService, toastrService: ToastrService) {
    super(boardGamesService, toastrService);
  }
}
