// events.service.ts
// Author: David Chocholatý

import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Event } from "../../models/events/event.model";
import { ClubState } from "../../models/states/club-state.model";
import { formatDate } from "@angular/common";

@Injectable({
  providedIn: 'root'
})
export class EventsService {
  readonly EventsUrl = environment.baseApiUrl + '/events';
  readonly StatesUrl = environment.baseApiUrl + '/states';

  constructor(
    private http: HttpClient,
    private toastr: ToastrService,
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

  getFromToEvents(from: string = "", to: string = ""): Observable<Event[]> {
    let params = new HttpParams();
    if (from != "") {
      params = params.set("from", from);
    }
    if (to != "") {
      params = params.set("to", to);
    }

    return this.http.get<Event[]>(this.EventsUrl, {params: params});
  }

  getMonthEvents(month: Date, peekNextMonth: boolean = true): Observable<Event[]> {
    let firstDay = new Date(month.getFullYear(), month.getMonth(), month.getDay());
    firstDay.setDate(1);
    let lastDay;
    if (peekNextMonth) {
      // 35 is the number of days to show when presenting a calendar
      // firstDay is 00:00:00, so last day is firstDay + 36 days - 1 second (let's not care about leap seconds and DST...)
      lastDay = new Date(firstDay.getTime() + 36 * 86400000 - 1);
    } else {
      lastDay = new Date(month.getFullYear(), month.getMonth() + 1, -1, 23, 59, 59);
    }
    return this.getFromToEvents(firstDay.toISOString(), lastDay.toISOString());
  }

  getEvent(eventId: number, withLinkedStates: boolean = false): Observable<Event> {
    let params = new HttpParams().set('withLinkedStates', withLinkedStates);
    return this.http.get<Event>(`${this.EventsUrl}/${eventId}`, {params});
  }

  /**
   * Gets current events as a list of events.
   */
  getCurrentEvents(): Observable<Event[]> {
    return this.http.get<Event[]>(`${this.EventsUrl}/current`);
  }

  refreshCurrentEvents() {
    this.getCurrentEvents().toPromise()
      .then((res: Event[]) => {
        this.eventsList = res;
      }).catch((error: any) => {
      console.log(error);
      this.toastr.error("Nepodařilo se načíst aktuální akce.", "Načtení akcí");
      return;
    });
  }

  planEvent() {
    return this.http.post(this.EventsUrl, this.eventDetail);
  }

  modifyEvent() {
    return this.http.put(`${this.EventsUrl}/${this.eventDetail.id}`, this.eventDetail);
  }

  removeEvent(eventId: number): Observable<any> {
    return this.http.delete(`${this.EventsUrl}/${eventId}`);
  }

  refreshEventsList() {
    this.getFromToEvents().toPromise()
      .then((res: Event[]) => {
        this.eventsList = res;
      }).catch((error: any) => {
      console.log(error);
      this.toastr.error("Nepodařilo se načíst akce.", "Načtení akcí");
      return;
    });
  }

  populateForm(selectedEventDetail: Event) {
    this.eventDetail = Object.assign({}, selectedEventDetail);
  }

  handleRemoveEventRequest(eventDetail?: Event) {
    if (eventDetail) {
      this.eventDetail = Object.assign({}, eventDetail);
    }

    if (confirm("Opravdu chcete odstranit akci " + this.eventDetail.name + "?")) {
      this.removeEvent(this.eventDetail.id).subscribe(
        res => {
          this.refreshEventsList();
          this.toastr.success('Akce úspěšně zrušena.', 'Zrušení akce');
        },
        err => {
          console.log(err)
          this.toastr.error('Akci se nepovedlo zrušit.', 'Zrušení akce');
        }
      );
    }
  }

  public getEventData(eventId: number, withLinkedStates: boolean = false) {
    this.getEvent(eventId, withLinkedStates).subscribe(
      res => {
        this.eventDetail = res as Event;
      },
      err => {
        console.log(err);
        this.toastr.error(`Nepodařilo se načíst akci s ID ${eventId}.`, "Načtení akcí");
      }
    );
  }

  refreshLinkedStatesList(eventId: number) {
    this.getEventData(eventId, true);
  }

  unlinkLinkedState(linkedStateId: number) {
    this.unlinkLinkedStateRequest(linkedStateId).toPromise()
      .then(() => {
        this.refreshLinkedStatesList(this.eventDetail.id);
        this.toastr.success("Odebrání připojeného stavu proběhlo úspěšně.", "Odebrání připojených stavů");
      }).catch((error: any) => {
      this.toastr.error("Odebrání připojeného stavu selhalo.", "Obebrání připojených stavů");
      return throwError(error);
    });
  }

  unlinkLinkedStateRequest(linkedStateId: number) {
    return this.http.delete(`${this.StatesUrl}/${linkedStateId}/linkedEvent`);
  }

  unlinkAllLinkedStates() {
    if (confirm(`Opravdu si přejete odebrat všechny připojené stavy z akce ${this.eventDetail.name}?`)) {
      this.unlinkAllLinkedStatesRequest().toPromise()
        .then(() => {
          this.refreshLinkedStatesList(this.eventDetail.id);
          this.toastr.success("Odebrání všech připojených stavů proběhlo úspěšně.", "Odebrání připojených stavů");
        }).catch((error: any) => {
          console.log(error);
          this.toastr.error("Odebrání všech připojených stavů selhalo.", "Odebrání připojených stavů");
          return;
        }
      );
    }
  }

  private unlinkAllLinkedStatesRequest() {
    return this.http.delete(`${this.EventsUrl}/${this.eventDetail.id}/linkedStates`);
  }

  getConflictingStatesForEventRequest(eventId: number) {
    return this.http.get<ClubState[]>(`${this.EventsUrl}/${eventId}/conflictingStates`);
  }

  linkAllConflictingStates(conflictingStatesIds: number[]) {
    if (confirm(`Opravdu si přejete přidat všechny existující stavy k akci ${this.eventDetail.name}?`)) {
      this.linkAllConflictingStatesRequest(conflictingStatesIds).toPromise()
        .then(() => {
          this.refreshLinkedStatesList(this.eventDetail.id);
          this.refreshConflictingStatesList(this.eventDetail.id);
          this.toastr.success("Přidání všech existujících stavů proběhlo úspěšně.", "Přidání existujících stavů");
        }).catch((error: any) => {
          console.log(error);
          this.toastr.error("Přidání všech existujících stavů selhalo.", "Přidání existujících stavů");
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
        this.toastr.success("Přidání existujícího stavu proběhlo úspěšně.", "Přidání existujících stavů");
      }).catch((error: any) => {
        console.log(error);
        this.toastr.error("Přidání existujícího stavu selhalo.", "Přidání existujících stavů");
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
        this.toastr.error(`Načtení všech možných existujících stavů pro event ${this.eventDetail.name} selhalo.`, "Načtení existujících stavů");
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

  private getConflictingStatesIdsForEvent() {
    let ids = [];
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
      this.toastr.error("Odebrání připojeného stavu selhalo.", "Obebrání připojených stavů");
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
          this.toastr.error("Odebrání připojených stavů selhalo.", "Odebrání připojených stavů");
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

  getFormattedFromDate(format: string = "d. M. y HH:MM") {
    return formatDate(this.eventDetail.from, format, "cs-CZ")
  }

  getFormattedToDate(format: string = "d. M. y HH:MM") {
    return formatDate(this.eventDetail.to, format, "cs-CZ")
  }
}
