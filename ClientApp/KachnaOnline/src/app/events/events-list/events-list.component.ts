import { Component, OnInit } from '@angular/core';
import { Event } from "../../models/events/event.model";
import { EventsService } from "../../shared/services/events.service";
import { ToastrService } from "ngx-toastr";
import { Router } from "@angular/router";
import { AuthenticationService } from "../../shared/services/authentication.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
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
    this.eventsService.getYearEvents(this.now).subscribe(
      res => this.setEvents(res),
      err => {
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

  yearChanged(year: Date) {
    this.eventsService.getYearEvents(year).subscribe(
      res => this.setEvents(res),
      err => {
        this.toastrService.error("Stažení akcí selhalo.", "Stažení akcí");
        return throwError(err);
      });
  }

  setEvents(eventModels: Event[]): void {
    this.events = this.sortEvents(eventModels);
  }

  sortEvents(events: Event[]): Event[] {
    return events.sort((a, b) => a.from.getTime() - b.from.getTime());
  }
}
