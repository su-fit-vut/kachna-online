// board-game.component.ts
// Author: František Nečas

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BoardGame } from "../../models/board-games/board-game-model";
import { AuthenticationService } from "../../shared/services/authentication.service";

@Component({
  selector: 'app-board-game',
  templateUrl: './board-game.component.html',
  styleUrls: ['./board-game.component.css']
})
export class BoardGameComponent implements OnInit {
  @Input() boardGame: BoardGame
  @Output() onReservationUpdate: EventEmitter<BoardGame> = new EventEmitter()

  constructor(public authenticationService: AuthenticationService) { }

  ngOnInit(): void {
  }

  reservedInitial(): void {
    this.boardGame.toReserve = 1;
    this.onReservationUpdate.emit(this.boardGame);
  }

  reservedIncrement(): void {
    this.boardGame.toReserve++;
    this.onReservationUpdate.emit(this.boardGame);
  }

  reservedDecrement(): void {
    this.boardGame.toReserve--;
    this.onReservationUpdate.emit(this.boardGame);
  }
}
