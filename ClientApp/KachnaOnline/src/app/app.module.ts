import { environment } from './../environments/environment';
import { EventsService } from './shared/services/events.service';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AccountPopupComponent } from './navigation-bar/account-popup/account-popup.component';
import { UserComponent } from './user/user.component';
import { NavigationBarComponent } from './navigation-bar/navigation-bar.component';
import { EventsComponent } from './events/events.component';
import { CurrentEventsComponent } from './events/current-events/current-events.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { StatesComponent } from './states/states.component';
import { EventDetailComponent } from './events/event-detail/event-detail.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EventFormComponent } from './events/event-form/event-form.component';
import { HomeComponent } from './home/home.component';
import { EventsFromAllComponent } from './events/events-from-all/events-from-all.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { LoginComponent } from './user/login/login.component';
import { JwtModule } from '@auth0/angular-jwt';
import { LoggedInContentComponent } from './navigation-bar/account-popup/logged-in-content/logged-in-content.component';
import { LoggedOutContentComponent } from './navigation-bar/account-popup/logged-out-content/logged-out-content.component';
import { PlanEventsComponent } from './events/plan-events/plan-events.component';

export function tokenGetter() {
  return localStorage.getItem('authenticationToken');
}

@NgModule({
  declarations: [
    AppComponent,
    AccountPopupComponent,
    UserComponent,
    NavigationBarComponent,
    EventsComponent,
    CurrentEventsComponent,
    PageNotFoundComponent,
    StatesComponent,
    EventDetailComponent,
    EventFormComponent,
    HomeComponent,
    EventsFromAllComponent,
    LoginComponent,
    LoggedInContentComponent,
    LoggedOutContentComponent,
    PlanEventsComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(), // TODO: Change options?
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: [environment.baseApiUrlDomain /*, 'localhost:5000', 'localhost:5001'*/],
        //disallowedRoutes: [],
      },
    }),
  ],
  providers: [ EventsService, ],
  bootstrap: [AppComponent]
})
export class AppModule { }
