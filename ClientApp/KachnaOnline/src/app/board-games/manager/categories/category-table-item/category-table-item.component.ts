import { Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { BoardGameCategory } from "../../../../models/board-games/board-game-category.model";

@Component({
  selector: '[app-category-table-item]',
  templateUrl: './category-table-item.component.html',
  styleUrls: ['./category-table-item.component.css']
})
export class CategoryTableItemComponent implements OnInit {
  @Input() category: BoardGameCategory;
  @Input() gameCount: number;
  @Output() categoryClicked: EventEmitter<BoardGameCategory> = new EventEmitter();

  constructor() { }
  @HostListener("click") onclick() {
    this.categoryClicked.emit(this.category);
  }

  ngOnInit(): void {
  }

}
