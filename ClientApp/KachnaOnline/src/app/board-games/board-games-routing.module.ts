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
