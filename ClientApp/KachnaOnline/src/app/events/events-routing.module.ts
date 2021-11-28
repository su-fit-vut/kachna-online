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

import { EventsService } from '../shared/services/events.service';
import { BrowserModule } from '@angular/platform-browser';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AccountPopupComponent } from '../navigation-bar/account-popup/account-popup.component';
import { NavigationBarComponent } from '../navigation-bar/navigation-bar.component';
import { PageNotFoundComponent } from '../page-not-found/page-not-found.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EventFormComponent } from './event-form/event-form.component';
import { HomeComponent } from '../home/home.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { JwtModule } from '@auth0/angular-jwt';
import { LoggedInContentComponent } from '../navigation-bar/account-popup/logged-in-content/logged-in-content.component';
import { LoggedOutContentComponent } from '../navigation-bar/account-popup/logged-out-content/logged-out-content.component';
import { ForbiddenComponent } from '../forbidden/forbidden.component';
import { EventsListComponent } from "./events-list/events-list.component";
import { ManageLinkedStatesComponent } from "./linked-states/manage-linked-states/manage-linked-states.component";

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
              title: `${environment.siteName} | Seznam akcí`,
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
