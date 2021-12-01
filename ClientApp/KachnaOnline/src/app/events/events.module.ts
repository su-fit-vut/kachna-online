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
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { LinkedStatesComponent } from './linked-states/linked-states.component';
import { ManageLinkedStatesComponent } from './linked-states/manage-linked-states/manage-linked-states.component';
import { ConflictingStatesComponent } from './linked-states/conflicting-states/conflicting-states.component';
import { ComponentsModule } from "../shared/components/components.module";
import { EditEventsComponent } from './edit-events/edit-events.component';
import { NextEventsComponent } from './next-events/next-events.component';
import { StateLocPipe } from "../shared/pipes/state-loc.pipe";
import { PipesModule } from "../shared/pipes/pipes.module";


@NgModule({
  declarations: [
    EventsListComponent,
    EventFormComponent,
    EventsFromAllComponent,
    EventDetailComponent,
    CurrentEventsComponent,
    PlanEventsComponent,
    LinkedStatesComponent,
    ManageLinkedStatesComponent,
    ConflictingStatesComponent,
    EditEventsComponent,
    NextEventsComponent,
  ],
    imports: [
        CommonModule,
        EventsRoutingModule,
        NgbModule,
        FormsModule,
        ComponentsModule,
        PipesModule,
        ReactiveFormsModule
    ]
})
export class EventsModule {
}
