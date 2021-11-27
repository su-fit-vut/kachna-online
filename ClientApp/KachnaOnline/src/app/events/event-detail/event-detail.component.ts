// event-detail.component.ts
// Author: David Chocholatý

import { ToastrService } from 'ngx-toastr';
import { EventsService } from '../../shared/services/events.service';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { Event } from '../../models/events/event.model';
import { AuthenticationService } from "../../shared/services/authentication.service";

@Component({
  selector: 'app-event-detail',
  templateUrl: './event-detail.component.html',
  styleUrls: ['./event-detail.component.css']
})
export class EventDetailComponent implements OnInit {
  event: Event = new Event();
  activateEditEventModal: boolean = false;

  constructor(
    public eventsService: EventsService,
    private toastrService: ToastrService,
    private route: ActivatedRoute,
    public authenticationService: AuthenticationService,
    ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      let eventId = Number(params.get('eventId'));
      this.eventsService.getEventData(eventId);
    });
  }


  onModifyButtonClicked() {
    this.activateEditEventModal = true;
  }

  onCloseModalClicked() {
    this.activateEditEventModal = false;
  }

  onDeleteButtonClicked() {
    this.eventsService.handleRemoveEventRequest();
  }
}
