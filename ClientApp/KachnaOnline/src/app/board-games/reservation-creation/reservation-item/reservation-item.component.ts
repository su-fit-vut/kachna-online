import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BoardGame } from "../../../models/board-games/board-game.model";
import { BoardGamesService } from "../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: '[app-reservation-item]',
  templateUrl: './reservation-item.component.html',
  styleUrls: ['./reservation-item.component.css']
})
export class ReservationItemComponent implements OnInit {
  @Input() boardGame: BoardGame
  @Output() countUpdated: EventEmitter<BoardGame> = new EventEmitter()

  constructor() {
  }

  ngOnInit(): void {
  }

  reservedIncrement(): void {
   this.boardGame.toReserve++;
   this.countUpdated.emit(this.boardGame);
  }

  reservedDecrement(): void {
    this.boardGame.toReserve--;
    this.countUpdated.emit(this.boardGame);
  }
}
