// board-games-page.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { Observable, OperatorFunction } from "rxjs";
import { distinctUntilChanged, map } from "rxjs/operators";
import { BoardGame } from "../../models/board-games/board-game.model";
import { BoardGamesService } from "../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { NgbTypeaheadSelectItemEvent } from "@ng-bootstrap/ng-bootstrap";
import { FormControl } from "@angular/forms";
import { BoardGameCategory } from "../../models/board-games/board-game-category.model";
import { AuthenticationService } from "../../shared/services/authentication.service";
import { BoardGamesStoreService } from "../../shared/services/board-games-store.service";

@Component({
  selector: 'app-board-games-page',
  templateUrl: './board-games-page.component.html',
  styleUrls: ['./board-games-page.component.css']
})
export class BoardGamesPageComponent implements OnInit {
  // Filters
  public searchModel: any;
  players: number | undefined;
  availableOnly: boolean | undefined;
  categoryIds: number[] = [];

  currentReservation: Map<number, number> = new Map();

  // In order for filtering to fully function, 3 arrays of games are required, all of them are subsets
  // of boardGames. filteredBoardGames contains games filtered by backend (players, categories, available),
  // while shownGames also considers the name search. This is necessary to keep track of the currently
  // filtered games while the user uses the search bar (while avoiding querying the backend).
  boardGames: BoardGame[] = [];
  filteredGames: BoardGame[] = [];
  shownGames: BoardGame[] = [];

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService,
              public authenticationService: AuthenticationService, public storeService: BoardGamesStoreService) {
  }

  ngOnInit(): void {
    [this.players, this.availableOnly, this.categoryIds, this.currentReservation] =
      this.storeService.getBoardGamePageState();
    // Fetch all games right away to reduce the number of requests.
    this.fetchGames([], true);
  }

  ngOnDestroy(): void {
    this.storeService.saveBoardGamePageState(this.players, this.availableOnly,
      this.categoryIds, this.currentReservation);
  }

  onCategoryAdded(category: number): void {
    if (this.categoryIds.length == 0) {
      // Prepare for category-based filtering.
      this.filteredGames = this.boardGames.filter(g => g.category.id == category);
    } else {
      let addToFiltered = this.boardGames.filter(g => g.category.id == category);
      // The games have not been fetched yet.
      if (addToFiltered.length == 0) {
        this.boardGamesService.getBoardGames([category], this.players, this.availableOnly).subscribe(
          games => {
            this.filteredGames = this.filteredGames.concat(games[0]);
            this.shownGames = this.filteredGames;
          }
        )
      } else {
        this.filteredGames = this.filteredGames.concat(addToFiltered);
      }
    }
    this.shownGames = this.filteredGames;
    this.categoryIds.push(category)
  }

  onCategoryRemoved(category: number): void {
    this.categoryIds = this.categoryIds.filter(c => c != category);
    if (this.categoryIds.length == 0) {
      // We need to re-fetch. If other filters are applied this.boardGames may not have all games.
      this.fetchGames([], false, true);
      this.filteredGames = this.boardGames;
    } else {
      this.filteredGames = this.filteredGames.filter(g => g.category.id != category);
    }
    this.shownGames = this.filteredGames;
  }

  onAvailabilityUpdate(newAvailability: boolean): void {
    this.availableOnly = newAvailability ? newAvailability : undefined;
    this.fetchGames(this.categoryIds, false, true);
  }

  fetchGames(categoryIds: number[], init: boolean = false, resetGames: boolean = false): void {
    this.boardGamesService.getBoardGames(categoryIds, this.players, this.availableOnly).subscribe(
      games => {
        if (resetGames) {
          this.boardGames = [];
        }
        for (let gamesSet of games) {
          this.boardGames = this.boardGames.concat(gamesSet);
        }
        // Use cached filters on init
        if (init && this.categoryIds.length > 0) {
          this.filteredGames = this.boardGames.filter(g => this.categoryIds.includes(g.category.id));
        } else {
          this.filteredGames = this.boardGames;
        }
        this.shownGames = this.filteredGames;
      },
      err => {
        console.log(err);
        this.toastrService.error("Načtení deskových her selhalo.");
      });
  }

  onPlayersChange(value: number | undefined): void {
    this.players = value;
    this.fetchGames(this.categoryIds, false, true);
  }

  search: OperatorFunction<string, readonly string[]> = (text$: Observable<string>) =>
    text$.pipe(
      distinctUntilChanged(),
      map(term => {
        this.shownGames = this.filteredGames.filter(g => g.name.toLowerCase().indexOf(term.toLowerCase()) > -1)
        return this.shownGames.map(g => g.name).slice(0, 10);
      })
    )

  selectItem(payload: NgbTypeaheadSelectItemEvent): void {
    this.shownGames = this.shownGames.filter(g => payload.item == g.name);
  }

  onReservationUpdate(game: BoardGame): void {
    if (!game.toReserve) {
      this.currentReservation.delete(game.id);
    } else {
      this.currentReservation.set(game.id, game.toReserve);
    }
  }
}
