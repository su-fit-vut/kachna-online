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
import { ReservationTableComponent } from './reservations/reservation-table/reservation-table.component';
import { ManagerReservationsComponent } from './manager/manager-reservations/manager-reservations.component';
import { ReservationItemsComponent } from './reservation-details/reservation-items/reservation-items.component';
import { ManagerReservationDetailsComponent } from './manager/manager-reservation-details/manager-reservation-details.component';
import { ManagerButtonClusterComponent } from './reservation-details/reservation-details-item/manager-button-cluster/manager-button-cluster.component';
import { ReservationHistoryComponent } from './manager/reservation-history/reservation-history.component';
import { CategoriesComponent } from './manager/categories/categories.component';
import { CategoryUpdateComponent } from './manager/categories/category-update/category-update.component';
import { CategoryCreateComponent } from './manager/categories/category-create/category-create.component';
import { CategoryTableItemComponent } from './manager/categories/category-table-item/category-table-item.component';
import { ManagerBoardGamesComponent } from './manager/manager-board-games/manager-board-games.component';
import { ColorPickerModule } from "ngx-color-picker";
import { BoardGameUpdateComponent } from './manager/manager-board-games/board-game-update/board-game-update.component';
import { BoardGameCreateComponent } from './manager/manager-board-games/board-game-create/board-game-create.component';
import { BoardGameTableItemComponent } from './manager/manager-board-games/board-game-table-item/board-game-table-item.component';
import { BoardGameCreateFormComponent } from './manager/manager-board-games/board-game-create-form/board-game-create-form.component';
import { ManagerTableItemComponent } from './manager/manager-board-games/manager-table-item/manager-table-item.component';
import { ColorCodingComponent } from './reservations/color-coding/color-coding.component';


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
    ManagerButtonClusterComponent,
    ReservationHistoryComponent,
    CategoriesComponent,
    CategoryUpdateComponent,
    CategoryCreateComponent,
    CategoryTableItemComponent,
    ManagerBoardGamesComponent,
    BoardGameUpdateComponent,
    BoardGameCreateComponent,
    BoardGameTableItemComponent,
    BoardGameCreateFormComponent,
    ManagerTableItemComponent,
    ColorCodingComponent,
  ],
  imports: [
    CommonModule,
    BoardGamesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    ComponentsModule,
    ColorPickerModule
  ]
})
export class BoardGamesModule {
}
