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
import { AuthenticationService } from "../../shared/services/authentication.service";
import { Reservation } from "../../models/board-games/reservation.model";
import { EMPTY, forkJoin, Observable, of } from "rxjs";
import { ReservationEventType } from "../../models/board-games/reservation-item-event.model";
import { map, mergeMap } from "rxjs/operators";

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
  loggedInUser: number

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService,
              private router: Router, private storeService: BoardGamesStoreService,
              public authService: AuthenticationService) {
  }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getUserId();
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

  updateAllItemStates(reservation: Reservation, state: ReservationEventType): Observable<any> {
    let requests = [];
    for (let item of reservation.items) {
      requests.push(this.boardGamesService.updateReservationState(reservation.id, item.id, state));
    }
    return forkJoin(requests);
  }

  reserve(assign: boolean = false, handover: boolean = false): void {
    if (this.currentReservation.size == 0) {
      this.toastrService.error("Rezervace nesmí být prázdná.")
      return;
    }
    if (this.pageMode == BoardGamePageState.Normal && !handover) {
      this.boardGamesService.reserve(this.currentReservation, this.noteForm.value).subscribe(
        _ => {
          // Reset reservation and redirect
          this.storeService.resetSavedReservation();
          this.toastrService.success("Nyní, prosím, vyčkej, než si rezervaci vezme na starost někdo z SU, a " +
            "domluv se s ním na předání hry.", "Rezervace vytvořena", {timeOut: 7000});
          this.router.navigate(["/board-games/reservations"]).then();
        },
        err => {
          console.log(err);
          this.showConflictingGame(err);
        }
      )
    } else if (this.pageMode == BoardGamePageState.ManagerReservation || handover) {
      if (!this.forUser) {
        // Handover from the user-mode by a manager.
        this.forUser = this.loggedInUser;
      }
      this.boardGamesService.reserveForUser(this.currentReservation, this.noteForm.value, this.forUser).pipe(
        mergeMap((reservation) => {
          let obs = assign ? this.updateAllItemStates(reservation, ReservationEventType.Assigned) : of(null);
          return obs.pipe(map(assignedResult => ({reservation, assignedResult})));
        })
      ).pipe(
        mergeMap(({reservation, assignedResult}) => {
          let obs =  handover ? this.updateAllItemStates(reservation, ReservationEventType.HandedOver) : of(null);
          return obs.pipe(map(handedOverResult => ({reservation, assignedResult, handedOverResult})));
        })
      ).pipe(mergeMap(({reservation, assignedResult, handedOverResult}) => {
        this.storeService.resetSavedReservation();
        this.storeService.setPageMode(BoardGamePageState.Normal);
        this.toastrService.success("Rezervace pro uživatele byla vytvořena.");
        return this.router.navigate([`/board-games/manager/reservations/${reservation.id}`]);
      })).subscribe();
    }
  }
}
