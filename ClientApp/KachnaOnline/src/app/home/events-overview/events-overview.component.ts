import { Component, OnInit } from '@angular/core';
import { EventsService } from "../../shared/services/events.service";
import { StatesService } from "../../shared/services/states.service";
import { Event } from "../../models/events/event.model";
import { forkJoin, Observable } from "rxjs";
import { ClubStateTypes } from "../../models/states/club-state-types.model";
import { ClubState } from "../../models/states/club-state.model";
import { endWith } from "rxjs/operators";

@Component({
  selector: 'app-events-overview',
  templateUrl: './events-overview.component.html',
  styleUrls: ['./events-overview.component.css']
})
export class EventsOverviewComponent implements OnInit {

  constructor(private eventsService: EventsService, private stateService: StatesService) { }

  events: EventItem[] = [];
  states: StateItem[] = [];

  shownEvents: EventItem[] = [];
  shownStates: StateItem[] = [];

  currentMonth: Date;
  showPast: boolean = false;

  ngOnInit(): void {
    this.currentMonth = new Date();
    this.changeMonth(0);
  }

  changeMonth(delta: number) {
    this.currentMonth.setMonth(this.currentMonth.getMonth() + delta);

    let now = new Date();
    if ((this.currentMonth.getMonth() < now.getMonth() && this.currentMonth.getFullYear() == now.getFullYear())
      || this.currentMonth.getFullYear() < now.getFullYear()) {
      this.showPast = true;
    }

    this.eventsService.getMonthEvents(this.currentMonth, false).subscribe(res => this.makeEvents(res));
    this.stateService.getMonth(this.currentMonth, false).subscribe(res => this.makeStates(res));
  }

  makeStates(stateModels: ClubState[]): void {
    this.states = [];
    let waiting: Observable<any>[] = [];

    for (let s of stateModels) {
      let state: StateItem = {
        from: s.start,
        to: s.actualEnd ?? s.plannedEnd,
        type: s.state,
        eventName: null
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

  makeEvents(eventModels: Event[]): void {
    this.events = [];
    let waiting: Observable<any>[] = [];

    for (let e of eventModels) {
      let event: EventItem = {
        from: e.from,
        to: e.to,
        id: e.id,
        imageUrl: e.imageUrl,
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
}

class EventItem {
  title: string;
  from: Date;
  to: Date;
  id: number;
  place: string | null;
  placeUrl: string | null;
  stateTypes: string[] | null;
  shortDescription: string;
  imageUrl: string;
  multipleDays: boolean;
}

class StateItem {
  from: Date;
  to: Date;
  type: ClubStateTypes;
  eventName: string | null;
}
