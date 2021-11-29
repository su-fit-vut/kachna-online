// events-list.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { Event } from "../../models/events/event.model";
import { EventsService } from "../../shared/services/events.service";
import { ToastrService } from "ngx-toastr";
import { Router } from "@angular/router";
import { AuthenticationService } from "../../shared/services/authentication.service";

@Component({
  selector: 'app-events-list',
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.css']
})
export class EventsListComponent implements OnInit {

  constructor(
    public eventsService: EventsService,
    private toastrService: ToastrService,
    private router: Router,
    public authenticationService: AuthenticationService,
  ) { }

  ngOnInit(): void {
    this.eventsService.refreshEventsList();
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
