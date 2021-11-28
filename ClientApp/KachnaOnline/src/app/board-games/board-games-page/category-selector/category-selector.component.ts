// category-selector.component.ts
// Author: František Nečas

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BoardGamesService } from "../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { BoardGameCategory } from "../../../models/board-games/board-game-category.model";

@Component({
  selector: 'app-category-selector',
  templateUrl: './category-selector.component.html',
  styleUrls: ['./category-selector.component.css']
})
export class CategorySelectorComponent implements OnInit {
  @Input() initiallyEnabled: number[] = [];
  @Output() categoryAdded: EventEmitter<number> = new EventEmitter();
  @Output() categoryRemoved: EventEmitter<number> = new EventEmitter();

  categories: BoardGameCategory[]

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService) {
  }

  ngOnInit(): void {
    this.boardGamesService.getCategories().subscribe(
      categories => {
        this.categories = categories;
      },
      err => {
        console.log(err)
        this.toastrService.error("Načtení kategorií deskových her selhalo.")
      }
    )
  }

  onCategoryEnabled(category: number) {
    this.categoryAdded.emit(category);
  }

  onCategoryDisabled(category: number) {
    this.categoryRemoved.emit(category);
  }
}
