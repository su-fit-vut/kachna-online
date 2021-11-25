// event-form.component.ts
// Author: David Chocholatý

import { Event } from '../../models/event.model';
import { EventsService } from '../../shared/services/events.service';
import { Component, Input, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.css']
})
export class EventFormComponent implements OnInit {

  constructor(
    public eventsService: EventsService,
    private toastrService: ToastrService,
    ) { }

  @Input() editMode: boolean = false;
  jumbotronText: string = "Naplánovat akci";

  ngOnInit(): void {
    if (this.editMode) {
      this.jumbotronText = "Upravit akci";
    } else {
      this.eventsService.eventDetail = new Event();
    }
  }

  onSubmit(form: NgForm) {
    if (this.eventsService.eventDetail.id == -1) // FIXME: When cleared, ID will be replaced. Remove clear button altogether?
      this.planEventFromForm(form);
    else
      this.modifyEventFromForm(form);
  }

  planEventFromForm(form: NgForm) {
    this.eventsService.planEvent().subscribe(
      res => {
        this.clearForm(form);
        this.eventsService.refreshEventsList();
        this.toastrService.success('Akce úspěšně naplánována.', 'Naplánovat akci');
      },
      err => {
        console.log(err);
        this.toastrService.error('Naplánování akce selhalo.', 'Naplánovat akci');
      }
    );
  }

  modifyEventFromForm(form: NgForm) {
    this.eventsService.modifyEvent().subscribe(
      res => {
        this.clearForm(form);
        this.eventsService.refreshEventsList();
        this.toastrService.success('Akce úspěšně upravena.', 'Upravit akci');
      },
      err => {
        console.log(err);
        this.toastrService.error('Úprava akce selhala.', 'Upravit akci');
      }
    );
  }

  clearForm(form: NgForm) {
    form.form.reset();
    this.eventsService.eventDetail = new Event();
  }


}
