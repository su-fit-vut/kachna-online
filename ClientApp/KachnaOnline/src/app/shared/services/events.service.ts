import { UserService } from './user.service';
import { ToastrService } from 'ngx-toastr';
import { EventsComponent } from './../../events/events.component';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
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
    private userService: UserService,
    ) { }

  eventDetail: Event = new Event();
  eventsList: Event[];
  headers = new HttpHeaders().set('Authorization', `Bearer ${this.userService.authenticationToken}`)

  getNextPlannedEvents():Observable<Event[]> {
    return this.http.get<Event[]>(`${this.EventsUrl}/next`, { headers: this.headers});
  }

  getFromToEvents(from: string = "", to: string = ""):Observable<Event[]> {
    let params = new HttpParams()
      .set('from', from)
      .set('to', to);
      // TODO: Handle from or to being empty strings. Is it possible it is handled automatically? Probably.

    return this.http.get<Event[]>(this.EventsUrl, {params: params, headers: this.headers});
  }

  getEvent(eventId: number): Observable<Event> {
    return this.http.get<Event>(`${this.EventsUrl}/${eventId}`, { headers: this.headers});
  }

  planEvent() {
    return this.http.post(this.EventsUrl, this.eventDetail, { headers: this.headers});
  }

  modifyEvent() {
    return this.http.put(`${this.EventsUrl}/${this.eventDetail.id}`, this.eventDetail, { headers: this.headers});
  }

  removeEvent(eventId: number):Observable<any> {
    return this.http.delete(`${this.EventsUrl}/${eventId}`, { headers: this.headers});
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
