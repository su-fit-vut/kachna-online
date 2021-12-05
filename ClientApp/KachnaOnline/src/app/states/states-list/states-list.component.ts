// states-list-component.ts
// Author: Ondřej Ondryáš

import { Component, OnInit } from '@angular/core';
import { StatesService } from "../../shared/services/states.service";
import { ClubState } from "../../models/states/club-state.model";

@Component({
  selector: 'app-states-list',
  templateUrl: './states-list.component.html',
  styleUrls: ['./states-list.component.css']
})
export class StatesListComponent implements OnInit {

  states: ClubState[] = [];
  now: Date = new Date();

  isInFuture: boolean = true;
  loaded: boolean = false;
  hasError: boolean = false;
  sortingFromOldest: boolean = true;

  currentMonth: Date;

  constructor(private service: StatesService) {
  }

  ngOnInit(): void {
    this.monthChanged(new Date());
    console.info(this.now);
  }

  monthChanged(month: Date): void {
    this.currentMonth = month;
    this.isInFuture = month.getTime() >= (new Date()).setDate(0);

    this.hasError = false;
    this.loaded = false;
    this.states = [];

    this.service.getMonth(month, false).subscribe(res => {
        this.states = res;
        this.sort(this.sortingFromOldest);
      },
      _ => this.hasError = true,
      () => this.loaded = true);
  }

  deleteState(state: ClubState): void {
    this.service.delete(state.id).subscribe(_ => this.monthChanged(this.currentMonth));
  }

  sort(fromOldest: boolean) {
    this.sortingFromOldest = fromOldest;

    if (fromOldest) {
      this.states.sort((a, b) => a.start.getTime() - b.start.getTime());
    } else {
      this.states.sort((a, b) => b.start.getTime() - a.start.getTime());
    }
  }

}
