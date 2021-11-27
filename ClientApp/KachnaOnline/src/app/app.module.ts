// app.module.ts
// Author: David Chocholatý

import { environment } from '../environments/environment';
import { EventsService } from './shared/services/events.service';
import { LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AccountPopupComponent } from './navigation-bar/account-popup/account-popup.component';
import { NavigationBarComponent } from './navigation-bar/navigation-bar.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { EventDetailComponent } from './events/event-detail/event-detail.component';
import { HTTP_INTERCEPTORS, HttpClientModule, HttpRequest } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HomeComponent } from './home/home.component';
import { ChillzoneDetailsComponent } from './home/chillzone-details/chillzone-details.component';
import { EventsFromAllComponent } from './events/events-from-all/events-from-all.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { JwtModule } from '@auth0/angular-jwt';
import { LoggedInContentComponent } from './navigation-bar/account-popup/logged-in-content/logged-in-content.component';
import { LoggedOutContentComponent } from './navigation-bar/account-popup/logged-out-content/logged-out-content.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { UsersModule } from "./users/users.module";
import { EventsModule } from "./events/events.module";
import { StatesModule } from "./states/states.module";
import { BoardGamesModule } from "./board-games/board-games.module";
import { ComponentsModule } from "./shared/components/components.module";
import localeCs from '@angular/common/locales/cs';
import { registerLocaleData } from "@angular/common";
import { RepeatingStatesComponent } from './home/repeating-states/repeating-states.component';
import { TimeStrPipe } from './shared/pipes/time-str.pipe';
import { DayLocPipe } from './shared/pipes/day-loc.pipe';
import { StateLocPipe } from './shared/pipes/state-loc.pipe';
import { BarDetailsComponent } from './home/bar-details/bar-details.component';
import { PrestigeTableComponent } from './home/bar-details/prestige-table/prestige-table.component';
import { UpcomingOpeningsComponent } from './home/upcoming-openings/upcoming-openings.component';
import { JsonDateInterceptor } from "./shared/interceptors/json-date.interceptor";
import { EventsOverviewComponent } from './home/events-overview/events-overview.component';
import { MonthLocPipe } from "./shared/pipes/month-loc.pipe";

registerLocaleData(localeCs);

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
    PageNotFoundComponent,
    HomeComponent,
    LoggedInContentComponent,
    LoggedOutContentComponent,
    ForbiddenComponent,
    ChillzoneDetailsComponent,
    RepeatingStatesComponent,
    BarDetailsComponent,
    TimeStrPipe,
    DayLocPipe,
    MonthLocPipe,
    StateLocPipe,
    PrestigeTableComponent,
    UpcomingOpeningsComponent,
    EventsOverviewComponent,
  ],
  imports: [
    ComponentsModule,
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
        allowedDomains: [environment.baseApiUrlDomain, environment.kisApiUrlDomain],
        //disallowedRoutes: [],
      },
    }),
    AppRoutingModule,
  ],
  providers: [
    EventsService,
    {provide: LOCALE_ID, useValue: 'cs-CZ'},
    {provide: HTTP_INTERCEPTORS, useClass: JsonDateInterceptor, multi: true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
