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
    if (confirm("Opravdu chcete odstranit akci " + selectedEventDetail.name + "?")) {
      this.eventsService.removeEvent(selectedEventDetail.id).subscribe(
        res => {
          this.eventsService.refreshEventsList();
          this.toastrService.success('Akce úspěšně zrušena.', 'Zrušení akce'); //? TODO: Use error toastr to show deletion?
        },
        err => {
          console.log(err)
          this.toastrService.error('Akci se nepovedlo zrušit.', 'Zrušení akce');
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
