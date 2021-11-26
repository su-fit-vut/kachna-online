// board-games-routing.module.ts
// Author: František Nečas

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BoardGamesPageComponent } from "./board-games-page/board-games-page.component";
import { environment } from "../../environments/environment";

const routes: Routes = [
  {
    path: 'boardGames',
    component: BoardGamesPageComponent,
    data: {
      title: `${environment.siteName} | Deskové hry`,
      description: "Přehled deskových her"
    },
    children: [

    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BoardGamesRoutingModule {
}
