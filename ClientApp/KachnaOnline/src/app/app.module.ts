import { environment } from '../environments/environment';
import { EventsService } from './shared/services/events.service';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AccountPopupComponent } from './navigation-bar/account-popup/account-popup.component';
import { NavigationBarComponent } from './navigation-bar/navigation-bar.component';
import { CurrentEventsComponent } from './events/current-events/current-events.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { EventDetailComponent } from './events/event-detail/event-detail.component';
import {HttpClientModule, HttpRequest} from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EventFormComponent } from './events/event-form/event-form.component';
import { HomeComponent } from './home/home.component';
import { EventsFromAllComponent } from './events/events-from-all/events-from-all.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { JwtModule } from '@auth0/angular-jwt';
import { LoggedInContentComponent } from './navigation-bar/account-popup/logged-in-content/logged-in-content.component';
import { LoggedOutContentComponent } from './navigation-bar/account-popup/logged-out-content/logged-out-content.component';
import { PlanEventsComponent } from './events/plan-events/plan-events.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import {UsersModule} from "./users/users.module";
import { EventsModule} from "./events/events.module";
import {StatesModule} from "./states/states.module";
import {BoardGamesModule} from "./board-games/board-games.module";

export function tokenGetter(request?: HttpRequest<any>) {
  if (request != null) {
    if (request.url.includes(environment.kisApiUrl)) {
      return localStorage.getItem(environment.kisAccessTokenStorageName)
    }
  }

  return localStorage.getItem(environment.accessTokenStorageName);
}

@NgModule({
  declarations: [
    AppComponent,
    AccountPopupComponent,
    NavigationBarComponent,
    CurrentEventsComponent,
    PageNotFoundComponent,
    EventDetailComponent,
    EventFormComponent,
    HomeComponent,
    EventsFromAllComponent,
    LoggedInContentComponent,
    LoggedOutContentComponent,
    PlanEventsComponent,
    ForbiddenComponent,
  ],
  imports: [
    StatesModule,
    BoardGamesModule,
    UsersModule,
    EventsModule,
    BrowserModule,
    NgbModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(), // TODO: Change options?
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: [environment.baseApiUrlDomain, environment.kisApiUrlDomain/*, 'localhost:5001'*/],
        //disallowedRoutes: [],
      },
    }),
    AppRoutingModule,
  ],
  providers: [ EventsService, ],
  bootstrap: [AppComponent]
})
export class AppModule { }
