// board-games-page.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { Observable, OperatorFunction } from "rxjs";
import { debounceTime, distinctUntilChanged } from "rxjs/operators";
import { BoardGame } from "../../models/board-games/board-game-model";
import { BoardGamesService } from "../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'app-board-games-page',
  templateUrl: './board-games-page.component.html',
  styleUrls: ['./board-games-page.component.css']
})
export class BoardGamesPageComponent implements OnInit {
  currentReservation: {[id: number]: number} = {};
  currentReservationLength: number = 0;
  boardGames: BoardGame[] = [];

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService) {
  }

  ngOnInit(): void {
    this.boardGamesService.getBoardGames(undefined, undefined, undefined).subscribe(
      games => {
        this.boardGames = games;
      },
      err => {
        console.log(err);
        this.toastrService.error("Načtení deskových her selhalo");
      });
  }

  //search: OperatorFunction<string, readonly string[]> = (text$ : Observable<string>) =>
  //  text$.pipe(
  //    debounceTime(200),
  //    distinctUntilChanged(),
  //    map(term => term.length < 2 ? [])
  //  )

  onReservationUpdate(game: BoardGame): void {
    console.log(this.currentReservationLength);
    if (!game.toReserve) {
      delete this.currentReservation[game.id];
      this.currentReservationLength--;
    } else {
      this.currentReservation[game.id] = game.toReserve;
      this.currentReservationLength++;
    }
  }
}
