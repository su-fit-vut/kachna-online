// events-overview-calendar.component.ts
// Author: František Nečas

import { Component, OnInit, ViewChild } from '@angular/core';
import { CalendarOptions, EventClickArg, FullCalendarComponent } from "@fullcalendar/angular";
import { StatesService } from "../../../shared/services/states.service";
import { EventsService } from "../../../shared/services/events.service";
import { ClubStateTypes } from "../../../models/states/club-state-types.model";
import { ToastrService } from "ngx-toastr";
import { Router } from "@angular/router";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { StateModalComponent } from "../state-modal/state-modal.component";
import { LoaderService } from "../../../shared/services/loader.service";
import { forkJoin } from "rxjs";

@Component({
  selector: 'app-events-overview-calendar',
  templateUrl: './events-overview-calendar.component.html',
  styleUrls: ['./events-overview-calendar.component.css']
})
export class EventsOverviewCalendarComponent implements OnInit {
  @ViewChild('calendar') calendarComponent: FullCalendarComponent

  calendarOptions: CalendarOptions = {
    headerToolbar: {
      left: 'title',
      center: '',
      right: 'prev,next'
    },
    initialView: 'dayGridMonth',
    firstDay: 1,
    dayMaxEvents: true,
    weekends: true,
    locale: 'cs-CZ',
    themeSystem: 'bootstrap',
    datesSet: this.onDateChange.bind(this),
    displayEventEnd: true,
    editable: true, // this adds cursor pointer
    eventClick: this.eventClick.bind(this),
    moreLinkContent: function(arg) {
      let text = `+${arg.num} další`;
      if (arg.num > 4) {
        text += 'ch';
      }
      return {html: `<p>${text}</p>`}
    }
  }

  private statePrefix = "state-"
  private eventPrefix = "event-";

  constructor(private statesService: StatesService, private eventsService: EventsService,
              private toastrService: ToastrService, private router: Router, private modalService: NgbModal,
              private loaderService: LoaderService) {
  }

  ngOnInit(): void {
  }

  onDateChange(dateInfo: { start: Date, end: Date }): void {
    this.updateCalendar(dateInfo.start, dateInfo.end);
  }

  updateCalendar(start: Date, end: Date): void {
    forkJoin([this.eventsService.getBetween(start, end), this.statesService.getBetween(start, end)]).subscribe(data => {
      let api = this.calendarComponent.getApi();
      api.removeAllEvents();
      for (let event of data[0]) {
        api.addEvent({
          id: `${this.eventPrefix}${event.id}`,
          title: event.name,
          start: event.from,
          end: event.to,
          display: 'block',
          color: "#28a745"
        })
      }
      for (let state of data[1]) {
        let title = "";
        let color = "#2A72FF";

        if (state.state == ClubStateTypes.OpenChillzone) {
          color = "#063f61";
          title = "Chillzóna";
        } else if (state.state == ClubStateTypes.OpenBar) {
          color = "#561303";
          title = "Bar";
        } else if (state.state == ClubStateTypes.Private) {
          color = "#6500F2";
          title = "Soukromá akce";
        }

        api.addEvent({
          id: `${this.statePrefix}${state.id}`,
          title: title,
          start: state.start,
          end: state.actualEnd ?? state.plannedEnd,
          display: 'block',
          color: color,
        })
      }
    }, err => {
      console.log(err);
      this.toastrService.error("Načtení kalendáře selhalo.");
    });
  }

  eventClick(clickInfo: EventClickArg) {
    if (clickInfo.event.id.startsWith(this.eventPrefix)) {
      let eventId = clickInfo.event.id.replace(this.eventPrefix, "");
      this.router.navigate([`/events/${eventId}`]).then();
    } else if (clickInfo.event.id.startsWith(this.statePrefix)) {
      let stateId = clickInfo.event.id.replace(this.statePrefix, "");
      this.statesService.get(parseInt(stateId)).subscribe(state => {
        const modalRef = this.modalService.open(StateModalComponent);
        modalRef.componentInstance.state = state;
      })
    }
  }
}
