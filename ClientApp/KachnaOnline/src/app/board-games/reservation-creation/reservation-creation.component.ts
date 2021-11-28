// reserver.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { BoardGamesService } from "../../shared/services/board-games.service";
import { BoardGame } from "../../models/board-games/board-game-model";
import { ToastrService } from "ngx-toastr";
import { FormControl } from "@angular/forms";
import { Router } from "@angular/router";
import { HttpStatusCode } from "@angular/common/http";

@Component({
  selector: 'app-reserve',
  templateUrl: './reservation-creation.component.html',
  styleUrls: ['./reservation-creation.component.css']
})
export class ReservationCreationComponent implements OnInit {
  currentReservation: Map<number, number> = new Map();
  games: Map<number, BoardGame> = new Map();
  noteForm = new FormControl('');

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService,
              private router: Router) {
  }

  ngOnInit(): void {
    this.currentReservation = this.boardGamesService.getBoardGamePageState()[3];
    this.boardGamesService.getBoardGames([], undefined, undefined).subscribe(
      games => {
        for (let gameSet of games) {
          for (let game of gameSet) {
            game.toReserve = this.currentReservation.get(game.id) || 0;
            this.games.set(game.id, game);
          }
        }
      },
      err => {
        console.log(err);
        this.toastrService.error("Načtení deskových her v rezervaci selhalo.")
      }
    )
  }

  updateReservation(boardGame: BoardGame): void {
    if (boardGame.toReserve == 0) {
      this.currentReservation.delete(boardGame.id);
      if (this.currentReservation.size == 0) {
        this.router.navigate(['/board-games']).then();
      }
    } else {
      this.currentReservation.set(boardGame.id, boardGame.toReserve);
    }
  }

  reserve(): void {
    console.log("here");
    if (this.currentReservation.size == 0) {
      this.toastrService.error("Rezervace nesmí být prázdná.")
      return;
    }
    this.boardGamesService.reserve(this.currentReservation, this.noteForm.value).subscribe(
      _ => {
        // Reset reservation and redirect
        this.boardGamesService.resetSavedReservation();
        this.router.navigate(["/board-games/reservations"]).then();
      },
      err => {
        console.log(err);
        if (err.status == HttpStatusCode.Conflict) {
          // Show the board game which is not available anymore
          this.boardGamesService.getBoardGame(err.error).subscribe(
            game => {
              this.toastrService.error(`Hra ${game.name} již byla zarezervována jiným uživatelem.`);
            },
            error => {
              // Should not happen, handle just in case.
              console.log(err);
              this.toastrService.error(`Některá z her v rezervaci již byla zarezervována jiným uživatelem.`);
            }
          )
        }
      }
    )
  }
}
