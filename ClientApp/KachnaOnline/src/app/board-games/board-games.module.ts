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
import { ReservationCreationComponent } from './reservation-creation/reservation-creation.component';
import { ReservationItemComponent } from './reservation-creation/reservation-item/reservation-item.component';
import { ReservationsComponent } from './reservations/reservations.component';
import { BoardGameDetailsComponent } from './board-game-details/board-game-details.component';
import { ReservationButtonComponent } from './board-games-page/board-game-card/reservation-button/reservation-button.component';
import { ComponentsModule } from "../shared/components/components.module";
import { ReservationComponent } from './reservations/reservation-table/reservation/reservation.component';
import { ReservationDetailsComponent } from './reservation-details/reservation-details.component';
import { ReservationDetailsItemComponent } from './reservation-details/reservation-details-item/reservation-details-item.component';
import { ReservationDetailsItemNormalComponent } from './reservation-details/reservation-details-item/reservation-details-item-normal/reservation-details-item-normal.component';
import { ReservationDetailsItemXsComponent } from './reservation-details/reservation-details-item/reservation-details-item-xs/reservation-details-item-xs.component';
import { NumberSelectionComponent } from "../shared/components/number-selection/number-selection.component";
import { ReservationTableComponent } from './reservations/reservation-table/reservation-table.component';
import { ManagerReservationsComponent } from './manager/manager-reservations/manager-reservations.component';
import { ReservationItemsComponent } from './reservation-details/reservation-items/reservation-items.component';
import { ManagerReservationDetailsComponent } from './manager/manager-reservation-details/manager-reservation-details.component';


@NgModule({
  declarations: [
    BoardGamesPageComponent,
    BoardGameCardComponent,
    CategorySelectorComponent,
    CategoryComponent,
    ReservationCreationComponent,
    ReservationItemComponent,
    ReservationsComponent,
    BoardGameDetailsComponent,
    ReservationButtonComponent,
    ReservationComponent,
    ReservationDetailsComponent,
    ReservationDetailsItemComponent,
    ReservationDetailsItemNormalComponent,
    ReservationDetailsItemXsComponent,
    ReservationTableComponent,
    ManagerReservationsComponent,
    ReservationItemsComponent,
    ManagerReservationDetailsComponent,
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
