// board-game-card-card.component.ts
// Author: František Nečas

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BoardGame } from "../../../models/board-games/board-game.model";
import { AuthenticationService } from "../../../shared/services/authentication.service";

@Component({
  selector: 'app-board-game',
  templateUrl: './board-game-card.component.html',
  styleUrls: ['./board-game-card.component.css']
})
export class BoardGameCardComponent implements OnInit {
  @Input() boardGame: BoardGame
  @Input() initialReserved: number = 0;
  @Output() reservationUpdate: EventEmitter<BoardGame> = new EventEmitter()

  constructor(public authenticationService: AuthenticationService) {
  }

  ngOnInit(): void {
    this.boardGame.toReserve = this.initialReserved;
  }

  onCountChanged(newCount: number): void {
    this.boardGame.toReserve = newCount;
    this.reservationUpdate.emit(this.boardGame);
  }
}
