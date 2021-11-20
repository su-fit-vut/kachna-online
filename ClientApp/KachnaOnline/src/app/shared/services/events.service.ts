import { ToastrService } from 'ngx-toastr';
import { EventsComponent } from './../../events/events.component';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event } from "../../models/event.model";

@Injectable({
  providedIn: 'root'
})
export class EventsService {
  readonly ApiUrl = "http://localhost:5000"; // TODO: Use /api/...
  readonly EventsUrl = this.ApiUrl + '/events';

  constructor(
    private http:HttpClient,
    private toastrService: ToastrService,
    ) { }

  eventDetail: Event = new Event();
  eventsList: Event[];

  getNextPlannedEvents():Observable<any[]> {
    return this.http.get<any>(this.EventsUrl + '/next');
  }

  getFromToEvents(from: string = "", to: string = ""):Observable<Event[]> {
    let params = new HttpParams();
    if (from != "") {
      params.set('from', from);
    }
    if (to != "") {
      params.set('to', to);
    }

    return this.http.get<Event[]>(this.EventsUrl, {params});
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
    this.getFromToEvents().subscribe(
      res => {
        this.eventsList = res as Event[];
      },
      err => {
        console.log(err);
        this.toastrService.error("Nepodařilo se získat eventy.", "Načtení eventů");
      }
    );
  }
}
