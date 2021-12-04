// events-from-all.component.ts
// Author: David Chocholatý

import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { EventsService } from '../../shared/services/events.service';
import { Event } from '../../models/events/event.model';
import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from "../../shared/services/authentication.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

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
    public authenticationService: AuthenticationService,
    private _modalService: NgbModal,
  ) { }

  ngOnInit(): void {
    this.eventsService.refreshEventsList();
  }

  openEventDetail(eventDetail: Event) {
    this.router.navigate([`/events/${eventDetail.id}`]).then();
  }

  onDeleteButtonClicked(selectedEventDetail: Event) {
    this.eventsService.openEventDeletionConfirmationModal(selectedEventDetail);
  }

  onModifyButtonClicked(selectedEventDetail: Event) {
    this.router.navigate([`/events/${selectedEventDetail.id}/edit`]).then();
  }

  onCloseModalClicked() {
    this.eventsService.refreshEventsList();
  }
}
