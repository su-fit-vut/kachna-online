import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PlanStateComponent } from "./plan-state/plan-state.component";
import { environment } from "../../environments/environment";
import { StatesManagerGuard } from "./states-manager.guard";
import { StatesListComponent } from "./states-list/states-list.component";

const routes: Routes = [
  {
    path: 'states',
    children: [
      {
        path: 'change',
        component: PlanStateComponent,
        canActivate: [StatesManagerGuard],
        data: {
          title: `${environment.siteName} | Změnit stav`,
          description: "Plánování stavu klubu",
          planningNew: false
        }
      },
      {
        path: 'plan',
        component: PlanStateComponent,
        canActivate: [StatesManagerGuard],
        data: {
          title: `${environment.siteName} | Naplánovat stav`,
          description: "Plánování stavu klubu",
          planningNew: true
        }
      },
      {
        path: 'history',
        component: StatesListComponent,
        canActivate: [StatesManagerGuard],
        data: {
          title: `${environment.siteName} | Historie a plán stavů`,
          description: "Historie a plán stavů"
        }
      },
      {
        path: ':id',
        component: PlanStateComponent,
        canActivate: [StatesManagerGuard],
        data: {
          title: `${environment.siteName} | Detail stavu`,
          description: "Detail stavu"
        }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StatesRoutingModule {
}
