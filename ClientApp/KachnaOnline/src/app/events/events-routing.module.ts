// events-routing.module.ts
// Author: David Chocholatý

import { environment } from '../../environments/environment';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CurrentEventsComponent } from "./current-events/current-events.component";
import { EventsFromAllComponent } from "./events-from-all/events-from-all.component";
import { PlanEventsComponent } from "./plan-events/plan-events.component";
import { EventsManagerGuard } from "./events-manager.guard";
import { EventDetailComponent } from "./event-detail/event-detail.component";
import { EventsListComponent } from "./events-list/events-list.component";
import { ManageLinkedStatesComponent } from "./linked-states/manage-linked-states/manage-linked-states.component";
import { ConflictingStatesComponent } from "./linked-states/conflicting-states/conflicting-states.component";
import { EditEventsComponent } from "./edit-events/edit-events.component";
import { NextEventsComponent } from "./next-events/next-events.component";

const routes: Routes = [
  {
    path: 'events',
    children: [
      {
        path: '',
        children: [
          {
            path: 'current',
            component: CurrentEventsComponent,
            data: {
              title: `${environment.siteName} | Aktuální akce`,
              description: 'Přehled aktuálních akcí',
            }
          },
          {
            path: 'next',
            component: NextEventsComponent,
            data: {
              title: `${environment.siteName} | Nejbližší akce`,
              description: 'Přehled nejbližších akcí',
            }
          },
          {
            path: 'all',
            component: EventsFromAllComponent,
          },
          {
            path: 'plan',
            component: PlanEventsComponent,
            canActivate: [EventsManagerGuard],
            data: {
              title: `${environment.siteName} | Plánování akce`,
              description: 'Plánování nové akce',
            }
          },
          {
            path: 'list',
            component: EventsListComponent,
            data: {
              title: `${environment.siteName} | Přehled akcí`,
              description: 'Přehled akcí',
            }
          },
          {
            path: 'linked-states',
            component: ManageLinkedStatesComponent,
          },
          {
            path: ':eventId',
            pathMatch: 'full',
            component: EventDetailComponent,
            data: {
              title: `${environment.siteName} | Detail akce`,
              description: 'Detailní popis akce',
            },
            children: [
              {
                path: '',
                component: EventDetailComponent,
                children: [
                ]
              }
            ]
          },
          {
            path: ':eventId/linked-states',
            pathMatch: 'full',
            component: ManageLinkedStatesComponent,
            canActivate: [EventsManagerGuard],
            data: {
              title: `${environment.siteName} | Připojené stavy`,
              description: 'Správa připojených stavů',
            }
          },
          {
            path: ':eventId/conflicting-states',
            pathMatch: 'full',
            component: ConflictingStatesComponent,
            canActivate: [EventsManagerGuard],
            data: {
              title: `${environment.siteName} | Existující stavy`,
              description: 'Přidání existujících stavů',
            }
          },
          {
            path: ':eventId/edit',
            pathMatch: 'full',
            component: EditEventsComponent,
            canActivate: [EventsManagerGuard],
            data: {
              title: `${environment.siteName} | Úprava akce`,
              description: 'Úprava existující akce',
            }
          },
        ]
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EventsRoutingModule { }
