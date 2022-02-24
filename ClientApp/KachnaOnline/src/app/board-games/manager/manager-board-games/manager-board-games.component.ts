import { Component } from '@angular/core';
import { BoardGame } from "../../../models/board-games/board-game.model";
import { ActivatedRoute, Router } from "@angular/router";
import { BoardGamesService } from "../../../shared/services/board-games.service";
import { BoardGamesPageComponent } from "../../board-games-page/board-games-page.component";
import { BoardGamesStoreService } from "../../../shared/services/board-games-store.service";
import { ToastrService } from "ngx-toastr";
import { AuthenticationService } from "../../../shared/services/authentication.service";

// FIXME: Refactor common logic with the user view. Also table could be shown to the user.
@Component({
  selector: 'app-manager-board-games',
  templateUrl: './manager-board-games.component.html',
  styleUrls: ['./manager-board-games.component.css']
})
export class ManagerBoardGamesComponent extends BoardGamesPageComponent {
  boardGames: BoardGame[]

  constructor(router: Router, private activatedRoute: ActivatedRoute,
              boardGamesService: BoardGamesService, storeService: BoardGamesStoreService,
              toastrService: ToastrService, authenticationService: AuthenticationService) {
    super(boardGamesService, toastrService, authenticationService, storeService, router);
  }

  ngOnInit(): void {
    this.fetchGames([], true);
  }

  ngOnDestroy() {
  }

  onBoardGameClicked(game: BoardGame): void {
    this.router.navigate([`${game.id}`], {relativeTo: this.activatedRoute}).then();
  }
}
