import { EventsService } from '../../shared/services/events.service';
import { Component, OnInit } from '@angular/core';
import { Event } from "../../models/events/event.model";
import { Router } from "@angular/router";
import { AuthenticationService } from "../../shared/services/authentication.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { EventItem } from "../../home/events-overview/events-overview.component";
import { StatesService } from "../../shared/services/states.service";
import { forkJoin, Observable } from "rxjs";
import { ClubStateTypes } from "../../models/states/club-state-types.model";
import { endWith } from "rxjs/operators";
import { ToastrService } from "ngx-toastr";
import { UrlUtils } from "../../shared/utils/url-utils";

@Component({
  selector: 'app-current-events',
  templateUrl: './current-events.component.html',
  styleUrls: ['./current-events.component.css']
})
export class CurrentEventsComponent implements OnInit {
  currentEvents: EventItem[] = [];
  nextEvents: EventItem[] = [];
  shownNextEvents: EventItem[] = [];
  currentMonth: Date = new Date();
  loading: boolean = false;

  getImageUrl = UrlUtils.getAbsoluteImageUrl;

  constructor(
    public eventsService: EventsService,
    private stateService: StatesService,
    private toastrService: ToastrService,
    private router: Router,
    public authenticationService: AuthenticationService,
    private _modalService: NgbModal,
  ) { }


  ngOnInit(): void {
    this.loading = true;
    this.eventsService.getCurrentEventsRequest().subscribe(
      (res: Event[]) => {
        this.makeCurrentEvents(res);
      },
      (_) => {
        this.toastrService.error("Nepodařilo se stáhnout aktuální akce.", "Stažení akcí")
      }
    );

    this.eventsService.getMonthEvents(new Date(), false).subscribe(
      (res: Event[]) => {
        this.makeNextEvents(res);
      },
      (_) => {
        this.toastrService.error("Nepodařilo se stáhnout následující akce.", "Stažení akcí")
      }
    );
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

  makeCurrentEvents(eventModels: Event[]): void {
    this.currentEvents = [];
    let waiting: Observable<any>[] = [];

    for (let e of eventModels) {
      let event: EventItem = {
        from: e.from,
        to: e.to,
        id: e.id,
        imageUrl: e.imageUrl == "" ? null : e.imageUrl,
        url: e.url == "" ? null : e.url,
        title: e.name,
        shortDescription: e.shortDescription,
        stateTypes: null,
        place: e.place,
        placeUrl: e.placeUrl,
        multipleDays: (e.to.getTime() - e.from.getTime() <= 86400000)
      };

      if (e.linkedPlannedStateIds != undefined && e.linkedPlannedStateIds?.length > 0) {
        let requests = e.linkedPlannedStateIds.map(e => this.stateService.get(e));
        event.stateTypes = [];

        let reqJoin = forkJoin(requests);
        waiting.push(reqJoin);

        reqJoin.subscribe(a => {
          let hasChillzone = false;
          let hasBar = false;

          for (let cs of a) {
            if (cs.state == ClubStateTypes.OpenBar && !hasBar) {
              event.stateTypes?.push("otevřeno s barem");
              hasBar = true;
            } else if (cs.state == ClubStateTypes.OpenChillzone && !hasChillzone) {
              event.stateTypes?.push("chillzóna");
              hasChillzone = true;
            }
          }

          this.currentEvents.push(event);
        })
      } else {
        this.currentEvents.push(event);
      }
    }

    forkJoin(waiting).pipe(endWith(null)).subscribe(_ => {
      this.currentEvents = this.sortEvents(this.currentEvents);
      //this.loading = false;
    });
  }

  getNextEvents(from: Date = new Date()) {
    let firstDay = new Date(from.getFullYear(), from.getMonth(), from.getDate());
    let lastDay = new Date(firstDay.getTime() + 62 * 86400000 - 1);

    firstDay.setTime(firstDay.getTime() - firstDay.getTimezoneOffset() * 60000);
    lastDay.setTime(lastDay.getTime() - lastDay.getTimezoneOffset() * 60000);
    return this.eventsService.getBetween(firstDay, lastDay);
  }

  makeNextEvents(eventModels: Event[]): void {
    this.nextEvents = [];
    let waiting: Observable<any>[] = [];

    for (let e of eventModels) {
      let event: EventItem = {
        from: e.from,
        to: e.to,
        id: e.id,
        imageUrl: e.imageUrl == "" ? null : e.imageUrl,
        url: e.url == "" ? null : e.url,
        title: e.name,
        shortDescription: e.shortDescription,
        stateTypes: null,
        place: e.place,
        placeUrl: e.placeUrl,
        multipleDays: (e.to.getTime() - e.from.getTime() <= 86400000)
      };

      // Skip current events.
      let now = new Date();
      if (event.from <= now && event.to >= now) {
        continue;
      }

      if (e.linkedPlannedStateIds != undefined && e.linkedPlannedStateIds?.length > 0) {
        let requests = e.linkedPlannedStateIds.map(e => this.stateService.get(e));
        event.stateTypes = [];

        let reqJoin = forkJoin(requests);
        waiting.push(reqJoin);

        reqJoin.subscribe(a => {
          let hasChillzone = false;
          let hasBar = false;

          for (let cs of a) {
            if (cs.state == ClubStateTypes.OpenBar && !hasBar) {
              event.stateTypes?.push("otevřeno s barem");
              hasBar = true;
            } else if (cs.state == ClubStateTypes.OpenChillzone && !hasChillzone) {
              event.stateTypes?.push("chillzóna");
              hasChillzone = true;
            }
          }

          this.nextEvents.push(event);
        })
      } else {
        this.nextEvents.push(event);
      }
    }

    forkJoin(waiting).pipe(endWith(null)).subscribe(_ => {
      this.updateNextEvents();
      this.loading = false;
    });
  }

  sortEvents(events: EventItem[]): EventItem[] {
    return events.sort((a, b) => a.from.getTime() - b.from.getTime());
  }

  monthChanged(month: Date) {
    this.eventsService.getMonthEvents(month, false).subscribe(res => this.makeNextEvents(res));
  }

  updateNextEvents(): void {
    this.update(this.nextEvents, this.shownNextEvents);
    this.shownNextEvents = this.sortEvents(this.shownNextEvents)
  }

  update(source: { to: Date }[], target: { to: Date }[]): void {
    const now = new Date().getTime();
    target.length = 0;

    for (let e of source) {
      if (e.to.getTime() < now)
        continue;

      target.push(e);
    }
  }
}
