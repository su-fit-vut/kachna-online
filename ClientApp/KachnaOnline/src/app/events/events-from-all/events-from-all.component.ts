// event-from-all.component.ts
// Author: David Chocholatý

import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { EventsService } from '../../shared/services/events.service';
import { Event } from '../../models/event.model';
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

  populateForm(selectedEventDetail: Event) {
    this.eventsService.eventDetail = Object.assign({}, selectedEventDetail);
  }

  openEventDetail(eventDetail: Event) {
    this.router.navigate([`/events/${eventDetail.id}`]).then(() => null); // FIXME: Open in modal view.
  }

  onDeleteButtonClicked(selectedEventDetail: Event) {
    if (confirm("Do you want to delete event" + selectedEventDetail.name + "?")) {
      this.eventsService.removeEvent(selectedEventDetail.id).subscribe(
        res => {
          this.eventsService.refreshEventsList();
          this.toastrService.success('Event úspěšně zrušen.', 'Zrušení eventu'); //? TODO: Use error toastr to show deletion?
        },
        err => {
          console.log(err)
          this.toastrService.error('Event se nepovedlo zrušit.', 'Zrušení eventu');
        }

      );
    }
  }

  onModifyButtonClicked(selectedEventDetail: Event) {
    this.populateForm(selectedEventDetail);
    this.activateEditEventModal = true;
  }

  onCloseModalClicked() {
    this.activateEditEventModal = false;
    this.eventsService.refreshEventsList();
  }
}
