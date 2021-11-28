// edit-events.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { EventsService } from "../../shared/services/events.service";

@Component({
  selector: 'app-edit-events',
  templateUrl: './edit-events.component.html',
  styleUrls: ['./edit-events.component.css']
})
export class EditEventsComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    public eventsService: EventsService,
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      let eventId = Number(params.get('eventId'));
      this.eventsService.getEventData(eventId, true);
    });
  }

}
