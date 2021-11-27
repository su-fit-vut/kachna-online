// board-game-details.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { ToastrService } from "ngx-toastr";
import { BoardGamesService } from "../../shared/services/board-games.service";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { BoardGame } from "../../models/board-games/board-game-model";

@Component({
  selector: 'app-board-game-details',
  templateUrl: './board-game-details.component.html',
  styleUrls: ['./board-game-details.component.css']
})
export class BoardGameDetailsComponent implements OnInit {
  private routeSub: Subscription
  boardGame: BoardGame

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService,
              private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.routeSub = this.route.params.subscribe(params => {
      this.boardGamesService.getBoardGame(params['id']).subscribe(game => {
          this.boardGame = game;
        },
        err => {
          console.log(err);
          this.toastrService.error("Načtení hry selhalo");
        })
    })
  }

  ngOnDestroy() {
    this.routeSub.unsubscribe();
  }

}
