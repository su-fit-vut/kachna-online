import { EventsService } from './../../shared/services/events.service';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { Observable, of } from 'rxjs';
import { Event } from '../../models/event.model';
import { switchMap, } from 'rxjs/operators';

@Component({
  selector: 'app-event-detail',
  templateUrl: './event-detail.component.html',
  styleUrls: ['./event-detail.component.css']
})
export class EventDetailComponent implements OnInit {
  event: Event;

  constructor(
    public eventsService: EventsService,
    private route: ActivatedRoute,
    private router: Router
    ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.eventsService.getEvent(params.get('eventId'))
        .toPromise().then(res => {
            this.event = res as Event;
            console.log(this.event.id);
        })
      });
  }



}
