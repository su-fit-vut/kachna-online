import { EventsService } from '../../shared/services/events.service';
import { Event } from '../../models/event.model';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-events-from-all',
  templateUrl: './events-from-all.component.html',
  styleUrls: ['./events-from-all.component.css']
})
export class EventsFromAllComponent implements OnInit {

  constructor(
    public eventsService: EventsService,
  ) { }

  activateEditEventModal: boolean = false;

  ngOnInit(): void {
    this.eventsService.refreshEventsList();
  }

  populateForm(selectedEventDetail: Event) {
    this.eventsService.eventDetail = selectedEventDetail;
  }

  onDeleteButtonClicked(selectedEventDetail: Event) {
    if (confirm("Do you want to delete event" + selectedEventDetail.name + "?")) {
      this.eventsService.removeEvent(selectedEventDetail.id).subscribe(res => {
        this.eventsService.refreshEventsList();
      });
    }
  }

  onModifyButtonClicked(selectedEventDetail: Event) {
    this.populateForm(selectedEventDetail);
    this.activateEditEventModal = true;

  }

  onCloseModalClicked() {
    this.activateEditEventModal = false;
    this.eventsService.refreshEventsList();
  }
}
