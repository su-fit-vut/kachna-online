// reservation-details-item-normal.component.ts
// Author: František Nečas

import { Component, HostListener } from '@angular/core';
import { ReservationDetailsItemComponent } from "../reservation-details-item.component";
import { BoardGamesService } from "../../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: '[app-reservation-details-item-normal]',
  templateUrl: './reservation-details-item-normal.component.html',
  styleUrls: ['./reservation-details-item-normal.component.css']
})
export class ReservationDetailsItemNormalComponent extends ReservationDetailsItemComponent {
  constructor(boardGamesService: BoardGamesService, toastrService: ToastrService) {
    super(boardGamesService, toastrService);
  }
}
