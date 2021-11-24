import { environment } from '../../environments/environment';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {CurrentEventsComponent} from "./current-events/current-events.component";
import {EventsFromAllComponent} from "./events-from-all/events-from-all.component";
import {PlanEventsComponent} from "./plan-events/plan-events.component";
import {EventsManagerGuard} from "./events-manager.guard";
import {EventDetailComponent} from "./event-detail/event-detail.component";

import { EventsService } from '../shared/services/events.service';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from '../app-routing.module';
import { AppComponent } from '../app.component';
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

const routes: Routes = [
  {
    path: 'events',
    children: [
      {
        path: '', children: [ {
          path: 'current',
          component: CurrentEventsComponent,
        },
          {
            path: 'all',
            component: EventsFromAllComponent,
          },
          {
            path: 'plan',
            component: PlanEventsComponent,
            canActivate: [EventsManagerGuard],
          },
          {
            path: ':eventId',
            component: EventDetailComponent,
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
