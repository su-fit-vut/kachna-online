// board-games.module.ts
// Author: František Nečas

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BoardGamesRoutingModule } from './board-games-routing.module';
import { BoardGamesPageComponent } from './board-games-page/board-games-page.component';
import { BoardGameComponent } from './board-game/board-game.component';


@NgModule({
  declarations: [
    BoardGamesPageComponent,
    BoardGameComponent
  ],
  imports: [
    CommonModule,
    BoardGamesRoutingModule
  ]
})
export class BoardGamesModule { }
