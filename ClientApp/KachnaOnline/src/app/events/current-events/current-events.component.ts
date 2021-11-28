// current-events.component.ts
// Author: David Chocholatý

import { EventsService } from '../../shared/services/events.service';
import { Component, OnInit } from '@angular/core';
import { Event } from "../../models/events/event.model";
import { Router } from "@angular/router";
import { AuthenticationService } from "../../shared/services/authentication.service";

@Component({
  selector: 'app-current-events',
  templateUrl: './current-events.component.html',
  styleUrls: ['./current-events.component.css']
})
export class CurrentEventsComponent implements OnInit {

  constructor(
    public eventsService: EventsService,
    private router: Router,
    public authenticationService: AuthenticationService,
  ) { }

  activateEditEventModal: boolean = false;

  ngOnInit(): void {
    this.eventsService.refreshCurrentEvents();
  }


  openEventDetail(eventDetail: Event) {
    this.router.navigate([`/events/${eventDetail.id}`]).then(() => null);
  }

  onDeleteButtonClicked(selectedEventDetail: Event) {
    this.eventsService.handleRemoveEventRequest(selectedEventDetail);
  }

  onModifyButtonClicked(selectedEventDetail: Event) {
    this.eventsService.populateForm(selectedEventDetail);
    this.activateEditEventModal = true;
  }

  onCloseModalClicked() {
    this.activateEditEventModal = false;
    this.eventsService.refreshCurrentEvents();
  }

}
