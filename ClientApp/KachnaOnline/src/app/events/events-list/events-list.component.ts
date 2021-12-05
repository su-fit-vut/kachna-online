// events-list.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { Event } from "../../models/events/event.model";
import { EventsService } from "../../shared/services/events.service";
import { ToastrService } from "ngx-toastr";
import { Router } from "@angular/router";
import { AuthenticationService } from "../../shared/services/authentication.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { first } from "rxjs/operators";
import { throwError } from "rxjs";

@Component({
  selector: 'app-events-list',
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.css']
})
export class EventsListComponent implements OnInit {
  events: Event[] = [];
  maxShortDescriptionChars: number = 50;
  now: Date = new Date();

  constructor(
    public eventsService: EventsService,
    private toastrService: ToastrService,
    private router: Router,
    public authenticationService: AuthenticationService,
    private _modalService: NgbModal,
  ) { }

  ngOnInit(): void {
    let firstDayOfYear = new Date(this.now.getFullYear(), 0, 1, 0, 0, 0, 0);
    let lastDayOfYear = new Date(firstDayOfYear);
    lastDayOfYear.setFullYear(lastDayOfYear.getFullYear() + 1);
    this.eventsService.getBetween(firstDayOfYear, lastDayOfYear).toPromise()
      .then((res: Event[]) => {
        this.events = res.sort((a, b) => a.from.getTime() - b.from.getTime());
      }).catch(err => {
        this.toastrService.error("Stažení akcí selhalo.", "Stažení akcí");
        return throwError(err);
    });
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
}
