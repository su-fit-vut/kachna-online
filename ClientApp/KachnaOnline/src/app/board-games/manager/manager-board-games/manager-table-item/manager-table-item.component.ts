import { Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { BoardGame } from "../../../../models/board-games/board-game.model";

@Component({
  selector: '[app-manager-table-item]',
  templateUrl: './manager-table-item.component.html',
  styleUrls: ['./manager-table-item.component.css']
})
export class ManagerTableItemComponent implements OnInit {
  @Output() boardGameClicked: EventEmitter<BoardGame> = new EventEmitter();
  @Input() game: BoardGame;

  constructor() { }

  @HostListener("click") onclick() {
    this.boardGameClicked.emit(this.game);
  }

  ngOnInit(): void {
  }
}
