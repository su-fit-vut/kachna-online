// events.service.ts
// Author: David Chocholatý

import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Event } from "../../models/events/event.model";
import { ClubState } from "../../models/states/club-state.model";

@Injectable({
  providedIn: 'root'
})
export class EventsService {
  readonly EventsUrl = environment.baseApiUrl + '/events';

  constructor(
    private http: HttpClient,
    private toastrService: ToastrService,
  ) { }

  eventDetail: Event = new Event();
  eventsList: Event[];

  getNextPlannedEvents(): Observable<Event[]> {
    return this.http.get<Event[]>(`${this.EventsUrl}/next`);
  }

  getFromToEvents(from: string = "", to: string = ""): Observable<Event[]> {
    let params = new HttpParams()
      .set('from', from)
      .set('to', to);
    // TODO: Handle from or to being empty strings. Is it possible it is handled automatically? Probably.

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
    return this.http.get<Event>(`${this.EventsUrl}/${eventId}`, { params });
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
      .then((res) => {
        this.eventsList = res as Event[];
      }).catch((error: any) => {
      console.log(error);
      this.toastrService.error("Nepodařilo se získat eventy.", "Načtení eventů");
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
          this.toastrService.success('Akce úspěšně zrušena.', 'Zrušení akce');
        },
        err => {
          console.log(err)
          this.toastrService.error('Akci se nepovedlo zrušit.', 'Zrušení akce');
        }
      );
    }
  }

  public getEventData(eventId: number) {
    this.getEvent(eventId).subscribe(
      res => {
        this.eventDetail = res as Event;
      },
      err => {
        console.log(err);
        this.toastrService.error(`Nepodařilo se získat akci s ID ${eventId}.`, "Načtení akcí");
      }
    );
  }
}
