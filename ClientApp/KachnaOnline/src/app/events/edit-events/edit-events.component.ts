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

  eventsBackRoute: string = "..";

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      let eventId = Number(params.get('eventId'));
      this.eventsService.getEventData(eventId, true);
    });

    this.eventsBackRoute = this.eventsService.getBackRoute();
  }

  ngOnDestroy() {
    this.eventsService.resetBackRoute();
  }

}
