// events-overview-text.component.ts
// Author: Ondřej Ondryáš

import { Component, OnInit } from '@angular/core';
import { EventsService } from "../../../shared/services/events.service";
import { StatesService } from "../../../shared/services/states.service";
import { Event } from "../../../models/events/event.model";
import { forkJoin, Observable } from "rxjs";
import { ClubStateTypes } from "../../../models/states/club-state-types.model";
import { endWith } from "rxjs/operators";
import { ClubState } from "../../../models/states/club-state.model";
import { EventItem, StateItem } from "../events-overview.component";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { StateModalComponent } from "../state-modal/state-modal.component";
import { UrlUtils } from "../../../shared/utils/url-utils";

@Component({
  selector: 'app-events-overview-text',
  templateUrl: './events-overview-text.component.html',
  styleUrls: ['./events-overview-text.component.css', '../../home.component.css']
})
export class EventsOverviewTextComponent implements OnInit {
  events: EventItem[] = [];
  states: StateItem[] = [];

  shownEvents: EventItem[] = [];
  shownStates: StateItem[] = [];

  showPast: boolean = false;

  getImageUrl = UrlUtils.getAbsoluteImageUrl;

  constructor(private eventsService: EventsService, private stateService: StatesService,
              private modalService: NgbModal, private statesService: StatesService) {
  }

  ngOnInit(): void {
    this.monthChanged(new Date());
  }

  monthChanged(month: Date) {
    let now = new Date();
    if ((month.getMonth() < now.getMonth() && month.getFullYear() == now.getFullYear())
      || month.getFullYear() < now.getFullYear()) {
      this.showPast = true;
    }

    this.eventsService.getMonthEvents(month, false).subscribe(res => this.makeEvents(res));
    this.stateService.getMonth(month, false).subscribe(res => this.makeStates(res));
  }

  makeEvents(eventModels: Event[]): void {
    this.events = [];
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

          this.events.push(event);
        })
      } else {
        this.events.push(event);
      }
    }

    forkJoin(waiting).pipe(endWith(null)).subscribe(_ => this.updateShownEvents());
  }

  makeStates(stateModels: ClubState[]): void {
    this.states = [];
    let waiting: Observable<any>[] = [];

    for (let s of stateModels) {
      let state: StateItem = {
        id: s.id,
        from: s.start,
        to: s.actualEnd ?? s.plannedEnd,
        type: s.state,
        eventName: null,
        hasNote: (s.noteInternal || s.note) !== null
      };

      if (s.eventId != null) {
        let req = this.eventsService.getEventRequest(s.eventId);
        waiting.push(req);
        req.subscribe(e => {
          state.eventName = e.name;
          this.states.push(state);
        });
      } else {
        this.states.push(state);
      }
    }

    forkJoin(waiting).pipe(endWith(null)).subscribe(_ => this.updateShownStates());
  }

  updateAll(checkEvent: any): void {
    this.showPast = checkEvent.target.checked;
    this.updateShownEvents();
    this.updateShownStates();
  }

  updateShownEvents(): void {
    this.update(this.events, this.shownEvents);
    this.shownEvents.sort((a, b) => a.from.getTime() - b.from.getTime());
  }

  updateShownStates(): void {
    this.update(this.states, this.shownStates);
    this.shownStates.sort((a, b) => a.from.getTime() - b.from.getTime());
  }

  update(source: { to: Date }[], target: { to: Date }[]): void {
    const now = new Date().getTime();
    target.length = 0;

    for (let e of source) {
      if (!this.showPast && e.to.getTime() < now)
        continue;

      target.push(e);
    }
  }

  onStateClicked(state: StateItem) {
    this.statesService.get(state.id).subscribe(state => {
      const modalRef = this.modalService.open(StateModalComponent);
      modalRef.componentInstance.state = state;
    })
  }
}
