import { Component, OnInit } from '@angular/core';
import { CalendarOptions } from "@fullcalendar/angular";

@Component({
  selector: 'app-events-overview-calendar',
  templateUrl: './events-overview-calendar.component.html',
  styleUrls: ['./events-overview-calendar.component.css']
})
export class EventsOverviewCalendarComponent implements OnInit {
  calendarOptions: CalendarOptions = {
    headerToolbar: {
      left: 'title',
      center: '',
      right: 'prev,next'
    },
    initialView: 'dayGridMonth',
    expandRows: true,
    weekends: true,
    selectable: true,
    locale: 'cs-CZ',
    themeSystem: 'bootstrap'
  }

  constructor() { }

  ngOnInit(): void {
  }

}
