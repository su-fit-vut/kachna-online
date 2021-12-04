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
    private router: Router,
    public authenticationService: AuthenticationService,
    ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      let eventId = Number(params.get('eventId'));
      this.eventsService.getEventData(eventId, true);
    });
  }

  onModifyButtonClicked() {
    this.route.paramMap.subscribe(params => {
      let eventId = Number(params.get('eventId'));
      this.router.navigate([`/events/${eventId}/edit`]).then();
    });
  }

  onCloseModalClicked() {
    this.activateEditEventModal = false;
  }

  onDeleteButtonClicked() {
    this.eventsService.openEventDeletionConfirmationModal(this.eventsService.eventDetail);
  }

  onManageLinkedStatesClicked() {
    this.route.paramMap.subscribe(params => {
      let eventId = Number(params.get('eventId'));
      this.router.navigate([`/events/${eventId}/linked-states`]).then();
    });
  }
}
