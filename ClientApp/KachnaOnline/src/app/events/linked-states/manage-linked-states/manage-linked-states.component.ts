// manage-linked-states.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { EventsService } from "../../../shared/services/events.service";
import { ToastrService } from "ngx-toastr";
import { ActivatedRoute, Router } from "@angular/router";
import { Event } from "../../../models/events/event.model";
import { ClubState } from "../../../models/states/club-state.model";

@Component({
  selector: 'app-manage-linked-states',
  templateUrl: './manage-linked-states.component.html',
  styleUrls: ['./manage-linked-states.component.css']
})
export class ManageLinkedStatesComponent implements OnInit {
  constructor(
    public eventsService: EventsService,
    private toastrService: ToastrService,
    private router: Router,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      let eventId = Number(params.get('eventId'));
      this.eventsService.refreshLinkedStatesList(eventId);
    });
  }

  openClubStateDetail(stateDetail: ClubState) {
    this.router.navigate([`/states/${stateDetail.id}`]).then(() => null);
  }

  onUnlinkButtonClicked(linkedState: ClubState) {
    this.eventsService.unlinkLinkedState(linkedState.id);

  }

  onDeleteButtonClicked(linkedState: ClubState) {

  }


  onPlanNewClubStateclicked() {
    this.router.navigate([`/states/plan`]).then(() => null);
  }

  onAddFromExistingClubStates() {

  }

  onUnlinkAllButtonClicked() {
    this.eventsService.unlinkAllLinkedStates();

  }

  onDeleteAllButtonClicked() {
    for (let stateId of this.eventsService.eventDetail.linkedPlannedStateIds) {
      //this.statesService.deleteState(stateId); // TODO: Uncomment when implemented.
    }
  }
}
