// event-form.component.ts
// Author: David Chocholatý

import { Event } from '../../models/events/event.model';
import { EventsService } from '../../shared/services/events.service';
import { Component, Input, OnInit } from '@angular/core';
import { NgForm, FormControl, FormGroup } from "@angular/forms";
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from "@angular/router";
import { NgbCalendar, NgbDateNativeAdapter, NgbDateStruct, NgbTimeStruct } from "@ng-bootstrap/ng-bootstrap";
import { throwError } from "rxjs";
import { HttpStatusCode } from "@angular/common/http";
import { ImageUploadService } from "../../shared/services/image-upload.service";
import { EventModification } from "../../models/events/event-modification.model";

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.css']
})
export class EventFormComponent implements OnInit {

  constructor(
    public eventsService: EventsService,
    public calendar: NgbCalendar,
    private nativeDateAdapter: NgbDateNativeAdapter,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
    private imageUploadService: ImageUploadService,
    ) { }

  form = new FormGroup({
    id: new FormControl(-1),
    name: new FormControl(""),
    shortDescription: new FormControl(""),
    fullDescription: new FormControl(""),
    imageUrl: new FormControl(""),
    image: new FormGroup({
      file: new FormControl(undefined),
    }),
    place: new FormControl(""),
    placeUrl: new FormControl(""),
    url: new FormControl(""),
    fromDate: new FormControl(this.calendar.getToday()),
    fromTime: new FormControl({hour: new Date().getHours(), minute: new Date().getMinutes()}),
    toDate: new FormControl(this.calendar.getToday()),
    toTime: new FormControl({hour: new Date().getHours() + 1, minute: new Date().getMinutes()}),
  });

  @Input() editMode: boolean = false;
  jumbotronText: string = "Naplánovat akci";
  submitText: string = "Přidat akci";
  image: string = "";

  ngOnInit(): void {
    if (this.editMode) {
      this.jumbotronText = "Upravit akci";
      this.submitText = "Uložit změny";
      this.route.paramMap.subscribe(params => {
        let eventId = Number(params.get('eventId'));
        this.eventsService.getEventRequest(eventId, true).toPromise()
          .then((edittedEvent: Event) => {
            this.form.controls.id.setValue(edittedEvent.id);
            this.form.controls.name.setValue(edittedEvent.name);
            this.form.controls.shortDescription.setValue(edittedEvent.shortDescription);
            this.form.controls.fullDescription.setValue(edittedEvent.fullDescription);
            this.form.controls.imageUrl.setValue(edittedEvent.imageUrl);
            this.form.controls.place.setValue(edittedEvent.place);
            this.form.controls.placeUrl.setValue(edittedEvent.placeUrl);
            this.form.controls.url.setValue(edittedEvent.url);
            this.form.controls.fromDate.setValue(this.nativeDateAdapter.fromModel(edittedEvent.from));
            this.form.controls.fromTime.setValue({
              hour: edittedEvent.from.getHours(),
              minute: edittedEvent.from.getMinutes(),
            });
            this.form.controls.toDate.setValue(this.nativeDateAdapter.fromModel(edittedEvent.to));
            this.form.controls.toTime.setValue({
              hour: edittedEvent.to.getHours(),
              minute: edittedEvent.to.getMinutes(),
            });
          }).catch(err => {
            this.toastr.error("Stažení dat o akci se nezdařilo.", "Upravit akci");
            return throwError(err);
          });
      });
    } else {
      this.eventsService.eventDetail = new Event();
    }
  }

  ngOnChanges() {
  }

  onSubmit() {
    let eventData = new EventModification();
    const formVal = this.form.value;

    eventData.id = formVal.id;
    eventData.name = formVal.name;
    eventData.place = formVal.place;
    eventData.placeUrl = formVal.placeUrl;
    eventData.shortDescription = formVal.shortDescription;
    eventData.fullDescription = formVal.fullDescription;
    eventData.url = formVal.url;

    // Process date and time values.
    const from = this.joinDateTime(formVal.fromDate, formVal.fromTime);
    const to =  this.joinDateTime(formVal.toDate, formVal.toTime);
    if (!this.verifyDates(from, to)) {
      return;
    }
    eventData.from = this.dateTimeToString(formVal.fromDate, formVal.fromTime);
    eventData.to = this.dateTimeToString(formVal.toDate, formVal.toTime);

    // Process image.
    let image = this.form.value['image'];
    delete this.form.value['image'];
    if (image.file) {
      this.imageUploadService.postFile(image['file']).subscribe(data => {
        this.form.patchValue({imageUrl: data.url});
        eventData.imageUrl = this.form.value.imageUrl;

        if (this.editMode) { // FIXME: When cleared, ID will be replaced. Remove clear button altogether?
          this.modifyEvent(eventData);
        }
        else {
          this.planEvent(eventData);
        }
      }, err => {
        if (err.status == HttpStatusCode.Conflict) {
          this.form.patchValue({imageUrl: err.error.url});
          eventData.imageUrl = this.form.value.imageUrl;

          if (this.editMode) { // FIXME: When cleared, ID will be replaced. Remove clear button altogether?
            this.modifyEvent(eventData);
          }
          else {
            this.planEvent(eventData);
          }
        } else {
          this.toastr.error("Nepodařilo se nahrát obrázek na server.");
        }
      })
    } else {
      this.form.patchValue({imageUrl: null});
      eventData.imageUrl = this.form.value.imageUrl;

      if (this.editMode) { // FIXME: When cleared, ID will be replaced. Remove clear button altogether?
        this.modifyEvent(eventData);
      }
      else {
        this.planEvent(eventData);
      }
    }
  }

  private joinDateTime(date: NgbDateStruct, time: NgbTimeStruct) : Date | null {
    let dateObj = this.nativeDateAdapter.toModel(date);
    dateObj?.setHours(time.hour);
    dateObj?.setMinutes(time.minute);
    dateObj?.setSeconds(0);
    return dateObj;
  }

  private verifyDates(from: Date | null, to: Date | null): boolean {
    if (!from || !to) {
      this.toastr.error("Akce musí mít nastavený počátek i konec akce.", "Plánování akce")
      return false;
    }
    if (from.getTime() < Date.now()) {
      this.toastr.error("Akce nemůže začínat v minulosti. Upravte termín počátku akce.", "Plánování akce")
      return false;
    }
    if (from.getTime() < Date.now()) {
      this.toastr.error("Akce nemůže končit v minulosti. Upravte termín konce akce.", "Plánování akce")
      return false;
    }
    if (from >= to) {
      this.toastr.error("Akce nemůže začínat po jejím konci. Upravte termín počátku nebo konce akce.", "Plánování akce")
      return false;
    }

    return true;
  }

  planEvent(eventData: EventModification) {
    this.eventsService.planEventRequest(eventData).subscribe(
      res => {
        this.form.reset();
        this.eventsService.refreshEventsList();
        this.toastr.success('Akce úspěšně naplánována.', 'Naplánovat akci');
      },
      err => {
        console.log(err);
        this.toastr.error('Naplánování akce selhalo.', 'Naplánovat akci');
      }
    );
  }

  modifyEvent(eventData: EventModification) {
    let eventId = eventData.id;
    this.eventsService.modifyEventRequest(eventData).subscribe(
      res => {
        this.eventsService.refreshEventsList();
        this.toastr.success('Akce úspěšně upravena.', 'Upravit akci');
        this.router.navigate([`/events/${eventId}`]).then();
      },
      err => {
        console.log(err);
        this.toastr.error('Úprava akce selhala.', 'Upravit akci');
      }
    );
  }

  clearForm(form: NgForm) {
    form.form.reset();
    this.eventsService.eventDetail = new Event();
  }

  onManageLinkedStatesClicked() {
    this.route.paramMap.subscribe(params => {
      let eventId = Number(params.get('eventId'));
      this.router.navigate([`/events/${eventId}/linked-states`]).then();
    });
  }

  private dateTimeToString(date: NgbDateStruct, time: NgbTimeStruct) : string {
    let dateObj = this.nativeDateAdapter.toModel(date);
    dateObj?.setTime(dateObj?.getTime() - dateObj?.getTimezoneOffset());
    dateObj?.setHours(time.hour);
    dateObj?.setMinutes(time.minute);
    dateObj?.setSeconds(0);
    return dateObj?.toISOString() ?? "";
  }

  imageChanged(event: any): void {
    this.form.patchValue({image: {file: event.target.files.item(0)}});
  }
}
