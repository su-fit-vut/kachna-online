// event-detail.component.ts
// Author: David Chocholatý

import { ToastrService } from 'ngx-toastr';
import { EventsService } from '../../shared/services/events.service';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { Event } from '../../models/event.model';

@Component({
  selector: 'app-event-detail',
  templateUrl: './event-detail.component.html',
  styleUrls: ['./event-detail.component.css']
})
export class EventDetailComponent implements OnInit {
  event: Event = new Event();

  constructor(
    public eventsService: EventsService,
    private toastrService: ToastrService,
    private route: ActivatedRoute,
    ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      let eventId = Number(params.get('eventId'));
      this.eventsService.getEvent(eventId).subscribe(
        res => {
            this.event = res as Event;
        },
        err => {
          console.log(err);
          this.toastrService.error(`Nepodařilo se získat event s ID ${eventId}.`, "Načtení eventů");
        });
    });
  }
}
