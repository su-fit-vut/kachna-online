// events.service.ts
// Author: David Chocholatý

import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Event } from "../../models/events/event.model";

@Injectable({
  providedIn: 'root'
})
export class EventsService {
  readonly EventsUrl = environment.baseApiUrl + '/events';

  constructor(
    private http:HttpClient,
    private toastrService: ToastrService,
    ) { }

  eventDetail: Event = new Event();
  eventsList: Event[];

  getNextPlannedEvents():Observable<Event[]> {
    return this.http.get<Event[]>(`${this.EventsUrl}/next`);
  }

  getFromToEvents(from: string = "", to: string = ""): Observable<Event[]> {
    let params = new HttpParams()
      .set('from', from)
      .set('to', to);
      // TODO: Handle from or to being empty strings. Is it possible it is handled automatically? Probably.

    return this.http.get<Event[]>(this.EventsUrl, {params: params});
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

  removeEvent(eventId: number):Observable<any> {
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

  public getEventData(eventId: number, withLinkedStates: boolean = false) {
    this.getEvent(eventId, withLinkedStates).subscribe(
      res => {
        this.eventDetail = res as Event;
      },
      err => {
        console.log(err);
        this.toastrService.error(`Nepodařilo se získat akci s ID ${eventId}.`, "Načtení akcí");
      }
    );
  }

  refreshLinkedStatesList(eventId: number) {
    this.getEventData(eventId, true);
  }

  unlinkLinkedState(linkedStateId: number) {
    return this.http.delete(`${this.EventsUrl}/`);
  }

  unlinkAllLinkedStates() {
    this.unlinkAllLinkedStatesRequest().toPromise()
      .then(() => {
        this.refreshLinkedStatesList(this.eventDetail.id);
      }).catch((error: any) => {
        console.log(error);
        this.toastrService.error("Nepodařilo se získat eventy.", "Načtení eventů");
        return;
      }
    );
  }

  private unlinkAllLinkedStatesRequest() {
    return this.http.delete(`${this.EventsUrl}/${this.eventDetail.id}/linkedStates`);
  }
}
