import { Component, OnInit } from '@angular/core';
import { ClubStateTypes } from "../../models/states/club-state-types.model";
import { environment } from "../../../environments/environment";

@Component({
  selector: 'app-events-overview',
  templateUrl: './events-overview.component.html',
  styleUrls: ['./events-overview.component.css', '../home.component.css']
})
export class EventsOverviewComponent implements OnInit {

  constructor() {
  }

  calendarView: boolean = true;

  ngOnInit(): void {
    let openView = localStorage.getItem(environment.homePageViewStorageName);
    if (openView === null) {
      this.calendarView = true;
    } else {
      this.calendarView = JSON.parse(openView) === true;
    }
  }

  setCalendarView(enabled: boolean): void {
    localStorage.setItem(environment.homePageViewStorageName, `${enabled}`);
    this.calendarView = enabled;
  }
}

export class EventItem {
  title: string;
  from: Date;
  to: Date;
  id: number;
  place: string | null;
  placeUrl: string | null;
  stateTypes: string[] | null;
  shortDescription: string;
  imageUrl: string | null;
  url: string | null;
  multipleDays: boolean;
}

export class StateItem {
  id: number;
  from: Date;
  to: Date;
  type: ClubStateTypes;
  eventName: string | null;
  hasNote: boolean = false;
}
