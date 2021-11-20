import { EventsComponent } from './../../events/events.component';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event } from "../../models/event.model";

@Injectable({
  providedIn: 'root'
})
export class EventsService {
  readonly ApiUrl = "http://localhost:5000";
  readonly EventsUrl = this.ApiUrl + '/events';

  constructor(private http:HttpClient) { }

  eventDetail: Event = new Event();

  getNextPlannedEvents():Observable<any[]> {
    return this.http.get<any>(this.EventsUrl + '/next');
  }

  getEvent(val: any): Observable<Event> {
    return this.http.get<Event>(this.EventsUrl + '/' + val);
  }

  planEvent(val: any) {
    return this.http.post(this.EventsUrl, val);
  }

  ModifyEvent(eventId: number, modifiedEvent: any) {
    return this.http.put(this.EventsUrl + '/' + eventId, modifiedEvent);
  }

  RemoveEvent(eventId: any):Observable<any> {
    return this.http.delete(this.EventsUrl + '/' + eventId);
  }
}
