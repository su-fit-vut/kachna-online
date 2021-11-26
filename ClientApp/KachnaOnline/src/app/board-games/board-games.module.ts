// board-games.module.ts
// Author: František Nečas

import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap'
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { BoardGamesRoutingModule } from './board-games-routing.module';
import { BoardGamesPageComponent } from './board-games-page/board-games-page.component';
import { BoardGameComponent } from './board-game/board-game.component';
import { CategorySelectorComponent } from './board-games-page/category-selector/category-selector.component';
import { CategoryComponent } from './board-games-page/category-selector/category/category.component';
import { TogglableButtonComponent } from './board-games-page/togglable-button/togglable-button.component';


@NgModule({
  declarations: [
    BoardGamesPageComponent,
    BoardGameComponent,
    CategorySelectorComponent,
    CategoryComponent,
    TogglableButtonComponent,
  ],
  imports: [
    CommonModule,
    BoardGamesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule
  ]
})
export class BoardGamesModule {
}
