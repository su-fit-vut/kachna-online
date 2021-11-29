// board-games-routing.module.ts
// Author: František Nečas

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BoardGamesPageComponent } from "./board-games-page/board-games-page.component";
import { environment } from "../../environments/environment";
import { ReservationCreationComponent } from "./reservation-creation/reservation-creation.component";
import { UserLoggedInGuard } from "../users/user-logged-in.guard";
import { ReservationsComponent } from "./reservations/reservations.component";
import { BoardGameDetailsComponent } from "./board-game-details/board-game-details.component";
import { ReservationDetailsComponent } from "./reservation-details/reservation-details.component";
import { BoardGamesManagerGuard } from "./board-games-manager.guard";
import { ManagerReservationsComponent } from "./manager/manager-reservations/manager-reservations.component";
import { ManagerReservationDetailsComponent } from "./manager/manager-reservation-details/manager-reservation-details.component";
import { ReservationHistoryComponent } from "./manager/reservation-history/reservation-history.component";
import { CategoriesComponent } from "./manager/categories/categories.component";
import { CategoryCreateComponent } from "./manager/categories/category-create/category-create.component";
import { CategoryUpdateComponent } from "./manager/categories/category-update/category-update.component";
import { ManagerBoardGamesComponent } from "./manager/manager-board-games/manager-board-games.component";

const routes: Routes = [
  {
    path: 'board-games',
    children: [
      {
        path: '',
        component: BoardGamesPageComponent,
        data: {
          title: `${environment.siteName} | Deskové hry`,
          description: "Přehled deskových her"
        }
      },
      {
        path: 'reserve',
        component: ReservationCreationComponent,
        canActivate: [UserLoggedInGuard],
        data: {
          title: `${environment.siteName} | Dokončení rezervace`,
          description: "Dokončení rezervace deskových her"
        }
      },
      {
        path: 'reservations',
        canActivate: [UserLoggedInGuard],
        children: [
          {
            path: '',
            component: ReservationsComponent,
            data: {
              title: `${environment.siteName} | Moje rezervace`,
              description: "Seznam mých rezervací"
            }
          },
          {
            path: ':id',
            component: ReservationDetailsComponent,
            data: {
              title: `${environment.siteName} | Moje rezervace`,
              description: "Moje rezervace"
            }
          }
        ]
      },
      {
        path: ':id',
        component: BoardGameDetailsComponent,
        data: {
          title: `${environment.siteName} | Desková hra`,
          description: `Detail deskové hry`
        }
      },
      {
        path: 'manager',
        canActivate: [BoardGamesManagerGuard],
        runGuardsAndResolvers: 'always',
        children: [
          {
            path: 'games',
            component: ManagerBoardGamesComponent,
            data: {
              title: `${environment.siteName} | Deskové hry`,
              description: `Správa deskových her`
            }
          },
          {
            path: 'categories',
            children: [
              {
                path: '',
                component: CategoriesComponent,
                data: {
                  title: `${environment.siteName} | Kategorie`,
                  description: `Přehled kategorií`
                }
              },
              {
                path: 'create',
                component: CategoryCreateComponent,
                data: {
                  title: `${environment.siteName} | Tvorba kategorie`,
                  description: `Tvorba kategorie`
                }
              },
              {
                path: ':id',
                component: CategoryUpdateComponent,
                data: {
                  title: `${environment.siteName} | Správa kategorie`,
                  description: `Správa a úprava kategorie`
                }
              }
            ]
          },
          {
            path: 'reservations',
            children: [
              {
                path: '',
                component: ManagerReservationsComponent,
                data: {
                  title: `${environment.siteName} | Všechny rezervace`,
                  description: `Rezervace všech uživatelů`
                },
              },
              {
                path: ':id',
                children: [
                  {
                    path: '',
                    component: ManagerReservationDetailsComponent,
                    data: {
                      title: `${environment.siteName} | Uživatelská rezervace`,
                      description: `Konkrétní rezervace uživatele`
                    }
                  },
                  {
                    path: ':itemId',
                    component: ReservationHistoryComponent,
                    data: {
                      title: `${environment.siteName} | Historie rezervace`,
                      description: `Historie rezervace jedné hry`
                    }
                  }
                ]
              }
            ]
          }
        ]
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BoardGamesRoutingModule {
}
