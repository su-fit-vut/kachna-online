// board-games.module.ts
// Author: František Nečas

import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap'
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { BoardGamesRoutingModule } from './board-games-routing.module';
import { BoardGamesPageComponent } from './board-games-page/board-games-page.component';
import { BoardGameCardComponent } from './board-games-page/board-game-card/board-game-card.component';
import { CategorySelectorComponent } from './board-games-page/category-selector/category-selector.component';
import { CategoryComponent } from './board-games-page/category-selector/category/category.component';
import { TogglableButtonComponent } from './board-games-page/togglable-button/togglable-button.component';
import { ReservationCreationComponent } from './reservation-creation/reservation-creation.component';
import { ReservationItemComponent } from './reservation-creation/reservation-item/reservation-item.component';
import { ReservationsComponent } from './reservations/reservations.component';
import { BoardGameDetailsComponent } from './board-game-details/board-game-details.component';
import { ReservationButtonComponent } from './board-games-page/board-game-card/reservation-button/reservation-button.component';
import { ComponentsModule } from "../shared/components/components.module";


@NgModule({
  declarations: [
    BoardGamesPageComponent,
    BoardGameCardComponent,
    CategorySelectorComponent,
    CategoryComponent,
    TogglableButtonComponent,
    ReservationCreationComponent,
    ReservationItemComponent,
    ReservationsComponent,
    BoardGameDetailsComponent,
    ReservationButtonComponent,
  ],
  imports: [
    CommonModule,
    BoardGamesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    ComponentsModule
  ]
})
export class BoardGamesModule {
}
