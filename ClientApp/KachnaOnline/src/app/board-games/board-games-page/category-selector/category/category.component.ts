import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BoardGameCategory } from "../../../../models/board-games/category-model";

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent implements OnInit {
  @Input() category: BoardGameCategory
  @Output() categoryEnabled: EventEmitter<BoardGameCategory> = new EventEmitter();
  @Output() categoryDisabled: EventEmitter<BoardGameCategory> = new EventEmitter();

  constructor() {
  }

  ngOnInit(): void {
  }

  onCheckboxChange(e: any): void {
    if (e.target.checked) {
      this.categoryEnabled.emit(this.category);
    } else {
      this.categoryDisabled.emit(this.category);
    }
  }
}
