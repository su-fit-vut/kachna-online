// category-selector.component.ts
// Author: František Nečas

import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { BoardGamesService } from "../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { BoardGameCategory } from "../../../models/board-games/category-model";

@Component({
  selector: 'app-category-selector',
  templateUrl: './category-selector.component.html',
  styleUrls: ['./category-selector.component.css']
})
export class CategorySelectorComponent implements OnInit {
  @Output() categoryAdded: EventEmitter<BoardGameCategory> = new EventEmitter();
  @Output() categoryRemoved: EventEmitter<BoardGameCategory> = new EventEmitter();

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

  onCategoryEnabled(category: BoardGameCategory) {
    this.categoryAdded.emit(category);
  }

  onCategoryDisabled(category: BoardGameCategory) {
    this.categoryRemoved.emit(category);
  }
}
