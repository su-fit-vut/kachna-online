import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BoardGameCategory } from "../../../../models/board-games/board-game-category.model";

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent implements OnInit {
  @Input() category: BoardGameCategory
  @Input() startingValue: boolean = false;
  @Output() categoryEnabled: EventEmitter<number> = new EventEmitter();
  @Output() categoryDisabled: EventEmitter<number> = new EventEmitter();

  constructor() {
  }

  ngOnInit(): void {
  }

  onCheckboxChange(e: any): void {
    if (e.target.checked) {
      this.categoryEnabled.emit(this.category.id);
    } else {
      this.categoryDisabled.emit(this.category.id);
    }
  }
}
