import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { Event } from '../../models/event.model';
import { EventsService } from './../../shared/services/events.service';
import { Component, Input, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
//import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.css']
})
export class EventFormComponent implements OnInit {

  constructor(
    public eventsService: EventsService,
    // Toastr fro submission.
    ) { }




  jumbotronText: string = "Naplánovat event";
  editMode: boolean = false;

  ngOnInit(): void {
    if (this.editMode) {
      this.jumbotronText = "Upravit event";
    }
  }

  onSubmit(form: NgForm) {
    if (this.eventsService.eventDetail.id == -1)
      this.planEventFromForm(form);
    else
      this.modifyEventFromForm(form);
  }

  planEventFromForm(form: NgForm) {
    this.eventsService.planEvent().subscribe(
      res => {
        this.clearForm(form);
        // TODO: Add toastr successfull.
        //this.toastr.success('Event úspěšně naplánován.', 'Event Operation')
      },
      err => {
        console.log(err);
        // TODO: Add toastr error.
        //this.toastr.success('Event úspěšně naplánován.', 'Event Operation')
      }
    );
  }

  modifyEventFromForm(form: NgForm) {
    this.eventsService.modifyEvent(this.eventsService.eventDetail.id, form).subscribe(
      res => {
        this.clearForm(form);
        // TODO: Add toastr successfull.
        //this.toastr.info('Event úspěšně aktualizován.', 'Event operation')
      },
      err => {
        console.log(err);
        // TODO: Add toastr error.
        //this.toastr.info('Event úspěšně aktualizován.', 'Event operation')
      }
    );
  }


  clearForm(form: NgForm) {
    form.form.reset();
    this.eventsService.eventDetail = new Event();
  }


}
