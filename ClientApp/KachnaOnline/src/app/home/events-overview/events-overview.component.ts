// events-overview.component.ts
// Authors: František Nečas, Ondřej Ondryáš

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

  toggleModel: any = {
    disabled: false,
    value: null
  }

  ngOnInit(): void {
    let openView = localStorage.getItem(environment.homePageViewStorageName);
    if (openView === null) {
      this.toggleModel.value = true;
    } else {
      this.toggleModel.value = JSON.parse(openView) === true;
    }
  }

  onViewChange(newValue: boolean): void {
    localStorage.setItem(environment.homePageViewStorageName, `${newValue}`);
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
  from: Date;
  to: Date;
  type: ClubStateTypes;
  eventName: string | null;
}
