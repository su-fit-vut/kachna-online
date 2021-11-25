import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Event } from "../../models/event.model";

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

  getEvent(eventId: number): Observable<Event> {
    return this.http.get<Event>(`${this.EventsUrl}/${eventId}`);
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
}
