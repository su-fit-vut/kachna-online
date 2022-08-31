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
import { HTTP_INTERCEPTORS, HttpClientModule, HttpRequest } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HomeComponent } from './home/home.component';
import { ChillzoneDetailsComponent } from './home/chillzone-details/chillzone-details.component';
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
import { BarDetailsComponent } from './home/bar-details/bar-details.component';
import { PrestigeTableComponent } from './home/bar-details/prestige-table/prestige-table.component';
import { UpcomingOpeningsComponent } from './home/upcoming-openings/upcoming-openings.component';
import { JsonDateInterceptor } from "./shared/interceptors/json-date.interceptor";
import { EventsOverviewComponent } from './home/events-overview/events-overview.component';
import { PipesModule } from "./shared/pipes/pipes.module";
import { LoadingSpinnerComponent } from './shared/components/loading-spinner/loading-spinner.component';
import { LoadingInterceptor } from "./shared/interceptors/loading.interceptor";
import { ServiceWorkerModule } from '@angular/service-worker';
import { CurrentOfferComponent } from './home/current-offer/current-offer.component';
import { NgToggleModule } from "@nth-cloud/ng-toggle";
import { EventsOverviewTextComponent } from './home/events-overview/events-overview-text/events-overview-text.component';
import { FullCalendarModule } from "@fullcalendar/angular";
import dayGridPlugin from '@fullcalendar/daygrid';
import bootstrapPlugin from '@fullcalendar/bootstrap';
import { EventsOverviewCalendarComponent } from "./home/events-overview/events-overview-calendar/events-overview-calendar.component";
import { StateModalComponent } from './home/events-overview/state-modal/state-modal.component';
import { KisTokenExpiredInterceptor } from "./shared/interceptors/kis-token-expired.interceptor";

FullCalendarModule.registerPlugins([
  dayGridPlugin,
  bootstrapPlugin,
])

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
    PrestigeTableComponent,
    UpcomingOpeningsComponent,
    EventsOverviewComponent,
    LoadingSpinnerComponent,
    CurrentOfferComponent,
    EventsOverviewTextComponent,
    EventsOverviewCalendarComponent,
    StateModalComponent,
  ],
  imports: [
    ComponentsModule,
    PipesModule,
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
    NgToggleModule,
    FullCalendarModule,
    ToastrModule.forRoot({
      maxOpened: 5, // Remaining toasts are queued.
      preventDuplicates: true,
      countDuplicates: true,
      resetTimeoutOnDuplicate: true,
      timeOut: 3000, // In milliseconds.
      closeButton: true, // Show close button to signal that it can be dismissed
    }),
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: [environment.baseApiUrlDomain, environment.kisApiUrlDomain],
        //disallowedRoutes: [],
      },
    }),
    AppRoutingModule,
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: true,
      // Register the ServiceWorker as soon as the app is stable
      // or after 30 seconds (whichever comes first).
      registrationStrategy: 'registerWhenStable:30000'
    }),
  ],
  providers: [
    EventsService,
    {provide: LOCALE_ID, useValue: 'cs-CZ'},
    {provide: HTTP_INTERCEPTORS, useClass: JsonDateInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: KisTokenExpiredInterceptor, multi:true},
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi:true},
  ],
  exports: [
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
