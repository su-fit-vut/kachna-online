// manage-linked-states.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { EventsService } from "../../../shared/services/events.service";
import { ToastrService } from "ngx-toastr";
import { ActivatedRoute, Router } from "@angular/router";
import { ClubState } from "../../../models/states/club-state.model";
import { StatesService } from "../../../shared/services/states.service";
import { Event } from "../../../models/events/event.model";
import { throwError } from "rxjs";

@Component({
  selector: 'app-manage-linked-states',
  templateUrl: './manage-linked-states.component.html',
  styleUrls: ['./manage-linked-states.component.css']
})
export class ManageLinkedStatesComponent implements OnInit {
  constructor(
    public eventsService: EventsService,
    private statesService: StatesService,
    private toastrService: ToastrService,
    private router: Router,
    private route: ActivatedRoute,
  ) { }

  eventDetail: Event = new Event();

  ngOnInit(): void {
    this.refreshLinkedStatesList();
  }

  refreshLinkedStatesList() {
    this.route.paramMap.subscribe(params => {
      let eventId = Number(params.get('eventId'));
      this.eventsService.getEventRequest(eventId, true).toPromise()
        .then((eventDetail: Event) => {
          this.eventDetail = eventDetail;
        }).catch(err => {
          this.toastrService.error("Nepodařilo se načíst navázané stavy k akci.", "Načtení navázaných stavů");
          return throwError(err);
      });
    });
  }

  openClubStateDetail(stateDetail: ClubState) {
    this.router.navigate([`/states/${stateDetail.id}`]).then(() => null);
  }

  onUnlinkButtonClicked(linkedState: ClubState) {
    if (this.linkedStateHasStarted(linkedState)) {
      this.toastrService.error("Tento stav již začal, není ho proto možné odebrat.")
      return;
    }
    this.eventsService.unlinkLinkedStateRequest(linkedState.id).toPromise()
      .then(() => {
        this.refreshLinkedStatesList();
        this.toastrService.success("Odpojení navázaného stavu proběhlo úspěšně.", "Odpojení navázaných stavů");
      }).catch((error: any) => {
      this.toastrService.error("Odpojení navázaného stavu selhalo.", "Odpojení navázaných stavů");
      return throwError(error);
    });
  }

  onDeleteButtonClicked(linkedState: ClubState) {
    this.eventsService.openLinkedStateDeletionConfirmationModal(linkedState).subscribe(_ => {
      this.refreshLinkedStatesList();
    });
  }

  onPlanNewClubStateClicked() {
    this.router.navigate([`/states/plan`], {state: {event: this.eventDetail}}).then(() => null);
  }

  onAddFromExistingClubStates() {
    this.router.navigate([`/events/${this.eventDetail.id}/conflicting-states`]).then(() => null);
  }

  onUnlinkAllButtonClicked() {
    if (confirm(`Opravdu si přejete odpojit od akce ${this.eventDetail.name} všechny stavy, které jsou k ní nyní navázány? Tato operace dané stavy nezruší.`)) {
      this.eventsService.unlinkAllLinkedStatesRequest().toPromise()
        .then(() => {
          this.toastrService.success("Odpojení napojených stavů proběhlo úspěšně.", "Odpojení napojených stavů");
          this.refreshLinkedStatesList();
        }).catch((error: any) => {
          this.toastrService.error("Odpojení napojených stavů selhalo.", "Odpojení napojených stavů");
          return throwError(error);
        }
      );
    }
  }

  onDeleteAllButtonClicked() {
    if (this.eventDetail.linkedPlannedStateIds == null) {
      return;
    }

    for (let stateId of this.eventDetail.linkedPlannedStateIds) {
      this.eventsService.openLinkedStatesDeletionConfirmationModal(this.eventDetail).subscribe(_ => {
        this.refreshLinkedStatesList();
      });
    }
  }

  /**
   * Checks whether linked states has already started.
   * @param linkedState State to check.
   */
  linkedStateHasStarted(linkedState: ClubState): boolean {
    return linkedState.start.getTime() <= Date.now() + (3600 * 1000); // TODO: +1 hour because of timezone. Fix properly.
  }

  /**
   * Checks whether any linked state has already started.
   */
  anyLinkedStateAlreadyStarted(): boolean {
    for (let linkedState of this.eventDetail.linkedStatesDtos) {
      if (this.linkedStateHasStarted(linkedState)) {
        return true;
      }
    }
    return false;
  }
}
