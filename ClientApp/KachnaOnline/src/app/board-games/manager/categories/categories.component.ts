import { Component, OnInit } from '@angular/core';
import { BoardGamesService } from "../../../shared/services/board-games.service";
import { ActivatedRoute, Router } from "@angular/router";
import { BoardGameCategory } from "../../../models/board-games/board-game-category.model";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.css']
})
export class CategoriesComponent implements OnInit {
  categories: BoardGameCategory[] = [];
  boardGameCounts: Map<number, number> = new Map();

  constructor(private boardGamesService: BoardGamesService, private router: Router,
              private toastrService: ToastrService, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.boardGamesService.getCategories().subscribe(categories => {
      this.categories = categories;
    }, err => {
      console.log(err);
      this.toastrService.error("Načtení kategorií selhalo")
    })

    this.boardGamesService.getBoardGames([], undefined, undefined).subscribe(games => {
      for (let gamesSet of games) {
        for (let game of gamesSet) {
          this.boardGameCounts.set(game.category.id, (this.boardGameCounts.get(game.category.id) || 0) + 1);
        }
      }
    }, err => {
      console.log(err);
      this.toastrService.error("Načtení her pro kategori selhalo");
    })
  }

  routeToCategory(category: BoardGameCategory): void {
    this.router.navigate([`${category.id}`], {relativeTo: this.activatedRoute}).then();
  }
}
