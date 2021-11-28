import { Component, OnInit } from '@angular/core';
import { EventsService } from "../../shared/services/events.service";

@Component({
  selector: 'app-linked-states',
  templateUrl: './linked-states.component.html',
  styleUrls: ['./linked-states.component.css']
})
export class LinkedStatesComponent implements OnInit {

  constructor(
    public eventsService: EventsService,
  ) { }

  ngOnInit(): void {
  }

}
