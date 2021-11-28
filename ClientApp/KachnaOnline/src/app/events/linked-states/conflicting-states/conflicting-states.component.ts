// conflicting-states.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { EventsService } from "../../../shared/services/events.service";
import { ClubState } from "../../../models/states/club-state.model";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'app-conflicting-states',
  templateUrl: './conflicting-states.component.html',
  styleUrls: ['./conflicting-states.component.css']
})
export class ConflictingStatesComponent implements OnInit {
  constructor(
    public eventsService: EventsService,
    public router: Router,
    private toastr: ToastrService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      let eventId = Number(params.get('eventId'));
      this.eventsService.refreshConflictingStatesList(eventId);
    });
  }

  onPlanNewClubStateClicked() {
    this.router.navigate([`/states/plan`]).then(() => null);
  }

  openClubStateDetail(stateDetail: ClubState) {
    this.router.navigate([`/states/${stateDetail.id}`]).then(() => null);
  }

  onLinkConflictingClubState(conflictingState: ClubState) {
    this.eventsService.linkConflictingClubState(conflictingState);
  }

  onLinkAllConflictingClubStates() {
    this.eventsService.linkAllConflictingClubStates();
  }
}
