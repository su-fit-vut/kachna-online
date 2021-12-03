// reserver.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { BoardGamesService } from "../../shared/services/board-games.service";
import { BoardGame } from "../../models/board-games/board-game.model";
import { ToastrService } from "ngx-toastr";
import { FormControl } from "@angular/forms";
import { Router } from "@angular/router";
import { HttpStatusCode } from "@angular/common/http";
import { BoardGamePageState, BoardGamesStoreService } from "../../shared/services/board-games-store.service";
import { UserDetail } from "../../models/users/user.model";

@Component({
  selector: 'app-reserve',
  templateUrl: './reservation-creation.component.html',
  styleUrls: ['./reservation-creation.component.css']
})
export class ReservationCreationComponent implements OnInit {
  currentReservation: Map<number, number> = new Map();
  games: Map<number, BoardGame> = new Map();
  noteForm = new FormControl('');
  pageMode: BoardGamePageState;
  forUser: number

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService,
              private router: Router, private storeService: BoardGamesStoreService) {
  }

  ngOnInit(): void {
    this.currentReservation = this.storeService.getBoardGamePageState()[3];
    this.pageMode = this.storeService.getPageMode();
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

  public get mode(): typeof BoardGamePageState {
    return BoardGamePageState;
  }

  showConflictingGame(err: any) {
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
    } else {
      this.toastrService.error("Rezervace se nezdařila (možná příliš dlouhá poznámka?)");
    }
  }

  userSelected(event: UserDetail): void {
    this.forUser = event.id;
  }

  reserve(): void {
    if (this.currentReservation.size == 0) {
      this.toastrService.error("Rezervace nesmí být prázdná.")
      return;
    }
    if (this.pageMode == BoardGamePageState.Normal) {
      this.boardGamesService.reserve(this.currentReservation, this.noteForm.value).subscribe(
        _ => {
          // Reset reservation and redirect
          this.storeService.resetSavedReservation();
          this.toastrService.success("Rezervace byla vytvořena.")
          this.router.navigate(["/board-games/reservations"]).then();
        },
        err => {
          console.log(err);
          this.showConflictingGame(err);
        }
      )
    } else if (this.pageMode == BoardGamePageState.ManagerReservation) {
      if (!this.forUser) {
        this.toastrService.error("Uživatel, pro kterého je tato rezervace, musí být zvolen.");
      } else {
        this.boardGamesService.reserveForUser(this.currentReservation, this.noteForm.value, this.forUser).subscribe(
          reservation => {
            this.storeService.resetSavedReservation();
            this.storeService.setPageMode(BoardGamePageState.Normal);
            this.toastrService.success("Rezervace pro uživatele byla vytvořena.");
            this.router.navigate([`/board-games/manager/reservations/${reservation.id}`]).then();
          }, err => {
            console.log(err);
            this.showConflictingGame(err);
          })
      }
    }
  }
}
