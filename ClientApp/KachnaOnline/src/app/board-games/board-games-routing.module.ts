// board-games-routing.module.ts
// Author: František Nečas

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BoardGamesPageComponent } from "./board-games-page/board-games-page.component";
import { environment } from "../../environments/environment";
import { ReservationCreationComponent } from "./reservation-creation/reservation-creation.component";
import { UserLoggedInGuard } from "../users/user-logged-in.guard";
import { ReservationsComponent } from "./reservations/reservations.component";

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
        component: ReservationsComponent,
        canActivate: [UserLoggedInGuard],
        data: {
          title: `${environment.siteName} | Moje rezervace`,
          description: "Seznam mých rezervací"
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
