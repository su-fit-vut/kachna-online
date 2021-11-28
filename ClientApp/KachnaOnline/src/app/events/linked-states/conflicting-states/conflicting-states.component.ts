// conflicting-states.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { EventsService } from "../../../shared/services/events.service";
import { ClubState } from "../../../models/states/club-state.model";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { delay } from "rxjs/operators";

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

  ngOnDestroy(): void {
    //this.eventsService.saveConflictingStatesPageState(this.unlinkedOnly);
  }

  onUnlinkedOnlyChange(newUnlinkedOnlyValue: boolean): void {
    this.eventsService.unlinkedOnly = newUnlinkedOnlyValue;
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
    if (conflictingState.eventId) {
      if (confirm(`Stav je již navázán na jinou akci. Opravdu si přejete navázat stav na event?`)) {
        this.eventsService.relinkClubStateToEvent(conflictingState);
      }
    } else {
      this.eventsService.linkConflictingClubState(conflictingState);
    }
  }

  onLinkAllConflictingClubStates() {
    let linkedAlready = false;
    for (let conflictingState of this.eventsService.shownConflictingStatesList) {
      if (conflictingState.eventId) {
        linkedAlready = true;
        break;
      }
    }

    if (linkedAlready) {
      if (confirm(`Některé stavy jsou již navázány na jinou akci. Opravdu si přejete navázat stavy na event?`)) {
        this.eventsService.relinkAllConflictingClubStateToEvent();
      }
    } else {
      this.eventsService.linkAllConflictingClubStates();
    }
  }

}
