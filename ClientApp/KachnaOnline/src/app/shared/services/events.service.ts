import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Event } from "../../models/events/event.model";
import { ClubState } from "../../models/states/club-state.model";
import { formatDate } from "@angular/common";
import { EventModification } from "../../models/events/event-modification.model";
import { DeletionConfirmationModalComponent, DeletionType,
} from "../components/deletion-confirmation-modal/deletion-confirmation-modal.component";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { StatesService } from "./states.service";

@Injectable({
  providedIn: 'root'
})
export class EventsService {
  readonly EventsUrl = environment.baseApiUrl + '/events';
  readonly StatesUrl = environment.baseApiUrl + '/states';

  constructor(
    private http: HttpClient,
    private toastr: ToastrService,
    private _modalService: NgbModal,
    private statesService: StatesService,
  ) {
  }

  eventDetail: Event = new Event();
  eventsList: Event[] = [];
  conflictingStatesList: ClubState[] = [];
  shownConflictingStatesList: ClubState[] = [];
  unlinkedOnly: boolean;

  eventsBackRoute: string = "..";

  getNextPlannedEvents(): Observable<Event[]> {
    return this.http.get<Event[]>(`${this.EventsUrl}/next`);
  }

  /**
   * Gets events starting in interval specified by parameter up to maximal allowed length
   * (as specified by backend settings).
   * @param from Start of the interval.
   * @param to End of the interval.
   */
  getEventsInInterval(from?: string, to?: string): Observable<Event[]> {
    let params = new HttpParams();
    if (from) {
      params = params.set("from", from);
    }
    if (to) {
      params = params.set("to", to);
    }

    return this.http.get<Event[]>(this.EventsUrl, {params: params});
  }

  getMonthEvents(month: Date, peekNextMonth: boolean = true): Observable<Event[]> {
    const year = month.getUTCFullYear();
    const monthNum = month.getUTCMonth();

    let firstDay = new Date(Date.UTC(year, monthNum, 1, 0, 0, 0));
    let lastDay;

    if (peekNextMonth) {
      // 35 is the number of days to show when presenting a calendar
      // firstDay is 00:00:00, so last day is firstDay + 36 days - 1 second (let's not care about leap seconds and DST...)
      lastDay = new Date(firstDay.getTime() + 36 * 86400000 - 1);
    } else {
      lastDay = new Date(Date.UTC(year, monthNum + 1, 1, 0, 0, 0));
    }

    return this.getBetween(firstDay, lastDay);
  }

  getYearEvents(year: Date) {
    let firstDayOfYear = new Date(year.getFullYear(), 0, 1, 0, 0, 0, 0);
    let lastDayOfYear = new Date(year.getFullYear(), 11, 31, 23, 59, 59);

    return this.getBetween(firstDayOfYear, lastDayOfYear);
  }

  getBetween(start: Date, end: Date): Observable<Event[]> {
    return this.getEventsInInterval(start.toISOString(), end.toISOString());
  }

  getEventRequest(eventId: number, withLinkedStates: boolean = false): Observable<Event> {
    let params = new HttpParams().set('withLinkedStates', withLinkedStates);
    return this.http.get<Event>(`${this.EventsUrl}/${eventId}`, {params});
  }

  /**
   * Gets current events as a list of events.
   */
  getCurrentEventsRequest(): Observable<Event[]> {
    return this.http.get<Event[]>(`${this.EventsUrl}/current`);
  }

  refreshCurrentEvents() {
    this.getCurrentEventsRequest().toPromise()
      .then((res: Event[]) => {
        this.eventsList = res;
      }).catch((error: any) => {
      console.log(error);
      this.toastr.error("Nepodařilo se načíst akce.", "Akce");
      return;
    });
  }

  planEventRequest(eventData: EventModification): Observable<any> {
    return this.http.post(this.EventsUrl, eventData);
  }

  modifyEventRequest(eventData: EventModification): Observable<any> {
    return this.http.put(`${this.EventsUrl}/${this.eventDetail.id}`, eventData);
  }

  removeEventRequest(eventId: number): Observable<any> {
    return this.http.delete(`${this.EventsUrl}/${eventId}`);
  }

  refreshEventsList() {
    this.getEventsInInterval().toPromise()
      .then((res: Event[]) => {
        this.eventsList = res;
      }).catch((error: any) => {
      console.log(error);
      this.toastr.error("Nepodařilo se načíst akce.", "Akce");
      return;
    });
  }

  populateForm(selectedEventDetail: Event) {
    this.eventDetail = Object.assign({}, selectedEventDetail);
  }

  handleRemoveEvent(eventDetail?: Event) {
    if (eventDetail) {
      this.eventDetail = Object.assign({}, eventDetail);
    }

    this.removeEventRequest(this.eventDetail.id).subscribe(
      res => {
        this.refreshEventsList();
        this.toastr.success("Akce úspěšně zrušena.", "Zrušení akce");
      },
      err => {
        console.log(err)
        this.toastr.error("Akci se nepovedlo zrušit.", "Zrušení akce");
      }
    );
  }

  public getEventData(eventId: number, withLinkedStates: boolean = false) {
    this.getEventRequest(eventId, withLinkedStates).subscribe(
      res => {
        this.eventDetail = res as Event;
      },
      err => {
        if (err.status && err.status === 404) {
          this.toastr.error("Hledaná akce neexistuje.", "Akce");
        } else {
          this.toastr.error("Nepodařilo se načíst informace o požadované akci.", "Akce");
        }
      }
    );
  }

  refreshLinkedStatesList(eventId: number) {
    this.getEventData(eventId, true);
  }

  unlinkLinkedStateRequest(linkedStateId: number) {
    return this.http.delete(`${this.StatesUrl}/${linkedStateId}/linkedEvent`);
  }

  unlinkAllLinkedStatesRequest() {
    return this.http.delete(`${this.EventsUrl}/${this.eventDetail.id}/linkedStates`);
  }

  getConflictingStatesForEventRequest(eventId: number) {
    return this.http.get<ClubState[]>(`${this.EventsUrl}/${eventId}/conflictingStates`);
  }

  linkAllConflictingStates(conflictingStatesIds: number[]) {
    if (confirm(`Opravdu si přejete navázat k akci ${this.eventDetail.name} všechny stavy, které jsou v jejím průběhu naplánovány?`)) {
      this.linkAllConflictingStatesRequest(conflictingStatesIds).toPromise()
        .then(() => {
          this.refreshLinkedStatesList(this.eventDetail.id);
          this.refreshConflictingStatesList(this.eventDetail.id);
          this.toastr.success("Navázání všech stavů proběhlo úspěšně.", "Navázání stavů k akci");
        }).catch((error: any) => {
          console.log(error);
          this.toastr.error("Navázání všech stavů selhalo.", "Navázání stavů k akci");
          return;
        }
      );
    }
  }

  private linkAllConflictingStatesRequest(conflictingStatesIds: number[]) {
    return this.http.post(`${this.EventsUrl}/${this.eventDetail.id}/linkedStates`, {"plannedStateIds": conflictingStatesIds});
  }

  linkConflictingState(conflictingStateId: number) {
    this.linkConflictingStateRequest(conflictingStateId).toPromise()
      .then(() => {
        this.refreshLinkedStatesList(this.eventDetail.id);
        this.refreshConflictingStatesList(this.eventDetail.id);
        this.toastr.success("Navázání stavu proběhlo úspěšně.", "Navázání stavu k akci");
      }).catch((error: any) => {
        console.log(error);
        this.toastr.error("Navázání stavu selhalo.", "Navázání stavu k akci");
      }
    );
  }

  private linkConflictingStateRequest(conflictingStateId: number) {
    return this.http.post(`${this.EventsUrl}/${this.eventDetail.id}/linkedStates`, {"plannedStateIds": [conflictingStateId]});
  }

  refreshConflictingStatesList(eventId: number) {
    this.getEventData(eventId, true);
    this.conflictingStatesList = [];
    this.getConflictingStatesForEvent(eventId);
  }

  getConflictingStatesForEvent(eventId: number) {
    this.getConflictingStatesForEventRequest(eventId).toPromise()
      .then((res: ClubState[]) => {
        for (let conflictingState of res) {
          if (this.eventDetail.linkedPlannedStateIds == null) {
            this.eventDetail.linkedPlannedStateIds = [];
          }

          if (!this.eventDetail.linkedPlannedStateIds.includes(conflictingState.id)
            && conflictingState.start.getTime() > Date.now() + (3600 * 1000)) { // TODO: +1 hour because of timezone. Fix properly.
            this.conflictingStatesList.push(conflictingState);
          }
        }
        this.shownConflictingStatesList = this.conflictingStatesList;
        this.filterConflictingStates(this.unlinkedOnly);
      }).catch((error: any) => {
        console.log(error);
        this.toastr.error(`Načtení stavů, které jsou naplánované v termínu akce ${this.eventDetail.name}, selhalo.`, "Akce");
      }
    );
  }

  linkConflictingClubState(conflictingState: ClubState) {
    this.linkConflictingState(conflictingState.id);
  }

  linkAllConflictingClubStates() {
    let ids = this.getConflictingStatesIdsForEvent();
    this.linkAllConflictingStates(ids);
  }

  private getConflictingStatesIdsForEvent(): number[] {
    let ids: number[] = [];
    for (let conflictingState of this.shownConflictingStatesList) {
      ids.push(conflictingState.id);
    }
    return ids;
  }

  saveBackRoute(route: string): void {
    this.eventsBackRoute = route;
  }

  getBackRoute(): string {
    return this.eventsBackRoute;
  }

  resetBackRoute(): void {
    this.eventsBackRoute = "..";
  }

  /**
   * Gets next events as a list of events.
   */
  getNextEvents(): Observable<Event[]> {
    return this.http.get<Event[]>(`${this.EventsUrl}/next`);
  }

  refreshNextEvents() {
    this.getNextEvents().toPromise()
      .then((res: Event[]) => {
        this.eventsList = res;
      }).catch((error: any) => {
      console.log(error);
      this.toastr.error("Nepodařilo se načíst nejbližší akce.", "Načtení akcí");
      return;
    });
  }

  /**
   * Return the saved state of conflicting states page.
   */
  getConflictingStatesPageState(): [boolean] {
    return [this.unlinkedOnly];
  }

  /**
   * Saves the current conflicting states page state (filters) so that the component can be destroyed and restored later.
   * @param unlinkedOnly Whether unlinked states only filtering is applied.
   */
  saveConflictingStatesPageState(unlinkedOnly: boolean): void {
    this.unlinkedOnly = unlinkedOnly;
  }

  relinkClubStateToEvent(conflictingState: ClubState) {
    this.unlinkLinkedStateRequest(conflictingState.id).toPromise()
      .then(() => {
        this.linkConflictingState(conflictingState.id);
      }).catch((error: any) => {
      this.toastr.error("Přepojování stavu selhalo.", "Navázání stavu k akci");
      return throwError(error);
    });
  }

  relinkAllConflictingClubStateToEvent() {
    for (let conflictingState of this.shownConflictingStatesList) {
      this.unlinkLinkedStateRequest(conflictingState.id).toPromise()
        .then(() => {
          this.linkConflictingState(conflictingState.id);
        }).catch((error: any) => {
          console.log(error);
          this.toastr.error("Přepojování stavů k jiné akci selhalo.", "Navázání stavů k akci");
          return;
        }
      );
    }
  }

  private filterConflictingStates(unlinkedOnly: boolean) {
    this.shownConflictingStatesList = this.conflictingStatesList.filter(
      conflictingState => {
        return (unlinkedOnly) ? conflictingState.eventId == null : true;
      });
  }

  getFormattedFromDate(format: string = "d. M. yyyy HH:mm") {
    return formatDate(this.eventDetail.from, format, "cs-CZ")
  }

  getFormattedToDate(format: string = "d. M. yyyy HH:mm") {
    return formatDate(this.eventDetail.to, format, "cs-CZ")
  }

  openEventDeletionConfirmationModal(eventDetail: Event) {
    const modal = this._modalService.open(DeletionConfirmationModalComponent);
    modal.componentInstance.name = eventDetail.name;
    modal.componentInstance.type = DeletionType.Event;

    modal.result.then(
      (result) => {
        if (result == "Confirm deletion") {
          this.handleRemoveEvent(eventDetail);
        }
      }, (reason) => {
      });
  }

  getLinkedStatesForEventRequest(eventId: number) {
    return this.http.get<ClubState[]>(`${this.EventsUrl}/${eventId}/linkedStates`);
  }

  openLinkedStateDeletionConfirmationModal(state: ClubState): Observable<any> {
    const modal = this._modalService.open(DeletionConfirmationModalComponent);
    modal.componentInstance.name = state.id;
    modal.componentInstance.type = DeletionType.LinkedState;

    modal.result.then(
      (result) => {
        if (result == "Confirm deletion") {
          return this.statesService.delete(state.id);
        }

        return new Observable<any>();
      }, (reason) => {
      });
    return new Observable<any>();
  }

  openLinkedStatesDeletionConfirmationModal(eventDetail: Event): Observable<any> {
    const modal = this._modalService.open(DeletionConfirmationModalComponent);
    modal.componentInstance.name = eventDetail.name;
    modal.componentInstance.type = DeletionType.LinkedStates;

    modal.result.then(
      (result) => {
        if (result == "Confirm deletion") {
          if (eventDetail.linkedPlannedStateIds) {
            for (let linkedStateId of eventDetail.linkedPlannedStateIds) {
              this.statesService.delete(linkedStateId).toPromise()
                .then(_ => {
                }).catch((error: any) => {
                  this.toastr.error(`Zrušení napojených stavů k akci ${eventDetail.name} selhalo.`, "Zrušení napojených stavů");
                  return throwError(error);
                });
            }
          }
        }

        return new Observable<any>();
      }, (reason) => {
      });
    return new Observable<any>();
  }
}
