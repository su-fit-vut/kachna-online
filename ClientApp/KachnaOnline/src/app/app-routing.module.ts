import { PlanEventsComponent } from './events/plan-events/plan-events.component';
import { LoginComponent } from './user-profile/login/login.component';
import { HomeComponent } from './home/home.component';
import { EventDetailComponent } from './events/event-detail/event-detail.component';
import { StatesComponent } from './states/states.component';
import { CurrentEventsComponent } from "./events/current-events/current-events.component";
import { PageNotFoundComponent } from "./page-not-found/page-not-found.component";
import { NgModule} from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EventsFromAllComponent } from './events/events-from-all/events-from-all.component';
import {UserProfileComponent} from "./user-profile/user-profile.component";

const routes: Routes = [
  /* Route order
The order of routes is important because the Router uses a first-match wins strategy when matching routes, so more specific routes should be placed above less specific routes. List routes with a static path first, followed by an empty path route, which matches the default route. The wildcard route comes last because it matches every URL and the Router selects it only if no other routes match first.
*/
  { path: 'login', component: LoginComponent },
  { path: 'user', component: UserProfileComponent },
  {
    path: 'events',
    children: [
      {
        path: '',
        children: [
          {
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
          },
          {
            path: ':eventId',
            component: EventDetailComponent,
          },
        ]
      }
    ]
  },
  { path: 'states', component: StatesComponent },

  { path: '', component: HomeComponent }, // Default home page.
  //{ path: '',   redirectTo: '/states', pathMatch: 'full' }, // Redirect to default states page.
  { path: '**', component: PageNotFoundComponent },  // Wildcard route for a 404 page.
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
