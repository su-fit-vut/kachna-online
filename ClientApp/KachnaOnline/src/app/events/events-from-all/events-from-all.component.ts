// events-from-all.component.ts
// Author: David Chocholatý

import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { EventsService } from '../../shared/services/events.service';
import { Event } from '../../models/events/event.model';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-events-from-all',
  templateUrl: './events-from-all.component.html',
  styleUrls: ['./events-from-all.component.css']
})
export class EventsFromAllComponent implements OnInit {

  constructor(
    public eventsService: EventsService,
    private toastrService: ToastrService,
    private router: Router,
  ) { }

  activateEditEventModal: boolean = false;

  ngOnInit(): void {
    this.eventsService.refreshEventsList();
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
    this.eventsService.refreshEventsList();
  }
}
