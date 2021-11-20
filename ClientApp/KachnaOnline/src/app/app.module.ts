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
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
