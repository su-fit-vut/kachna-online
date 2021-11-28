// reservation-details-item-xs.component.ts
// Author: František Nečas

import { Component } from '@angular/core';
import { ReservationDetailsItemComponent } from "../reservation-details-item.component";
import { BoardGamesService } from "../../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'app-reservation-details-item-xs',
  templateUrl: './reservation-details-item-xs.component.html',
  styleUrls: ['./reservation-details-item-xs.component.css']
})
export class ReservationDetailsItemXsComponent extends ReservationDetailsItemComponent {

  constructor(boardGamesService: BoardGamesService, toastrService: ToastrService) {
    super(boardGamesService, toastrService);
  }

}
