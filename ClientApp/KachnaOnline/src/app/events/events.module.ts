// events.module.ts
// Author: David Chocholatý

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EventsRoutingModule } from './events-routing.module';
import { EventsListComponent } from './events-list/events-list.component';
import { EventFormComponent } from "./event-form/event-form.component";
import { EventsFromAllComponent } from "./events-from-all/events-from-all.component";
import { EventDetailComponent } from "./event-detail/event-detail.component";
import { CurrentEventsComponent } from "./current-events/current-events.component";
import { PlanEventsComponent } from "./plan-events/plan-events.component";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { FormsModule } from "@angular/forms";
import { LinkedStatesComponent } from './linked-states/linked-states.component';


@NgModule({
  declarations: [
    EventsListComponent,
    EventFormComponent,
    EventsFromAllComponent,
    EventDetailComponent,
    CurrentEventsComponent,
    PlanEventsComponent,
    LinkedStatesComponent,
  ],
  imports: [
    CommonModule,
    EventsRoutingModule,
    NgbModule,
    FormsModule,
  ]
})
export class EventsModule { }
