// next-events.component.ts
// Author: David Chocholatý

import { EventsService } from '../../shared/services/events.service';
import { Component, OnInit } from '@angular/core';
import { Event } from "../../models/events/event.model";
import { Router } from "@angular/router";
import { AuthenticationService } from "../../shared/services/authentication.service";

@Component({
  selector: 'app-next-events',
  templateUrl: './next-events.component.html',
  styleUrls: ['./next-events.component.css']
})
export class NextEventsComponent implements OnInit {
  constructor(
    public eventsService: EventsService,
    private router: Router,
    public authenticationService: AuthenticationService,
  ) { }

  ngOnInit(): void {
    this.eventsService.refreshNextEvents();
  }

  openEventDetail(eventDetail: Event) {
    this.router.navigate([`/events/${eventDetail.id}`]).then();
  }

  onDeleteButtonClicked(selectedEventDetail: Event) {
    this.eventsService.handleRemoveEventRequest(selectedEventDetail);
  }

  onModifyButtonClicked(selectedEventDetail: Event) {
    this.router.navigate([`/events/${selectedEventDetail.id}/edit`]).then();
  }
}
