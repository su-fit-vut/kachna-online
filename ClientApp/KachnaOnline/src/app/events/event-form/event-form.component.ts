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

  mainForm = new FormGroup({
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
            this.mainForm.controls.id.setValue(edittedEvent.id);
            this.mainForm.controls.name.setValue(edittedEvent.name);
            this.mainForm.controls.shortDescription.setValue(edittedEvent.shortDescription);
            this.mainForm.controls.fullDescription.setValue(edittedEvent.fullDescription);
            this.mainForm.controls.imageUrl.setValue(edittedEvent.imageUrl);
            this.mainForm.controls.place.setValue(edittedEvent.place);
            this.mainForm.controls.placeUrl.setValue(edittedEvent.placeUrl);
            this.mainForm.controls.url.setValue(edittedEvent.url);
            this.mainForm.controls.fromDate.setValue(this.nativeDateAdapter.fromModel(edittedEvent.from));
            this.mainForm.controls.fromTime.setValue({
              hour: edittedEvent.from.getHours(),
              minute: edittedEvent.from.getMinutes(),
            });
            this.mainForm.controls.toDate.setValue(this.nativeDateAdapter.fromModel(edittedEvent.to));
            this.mainForm.controls.toTime.setValue({
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
    let eventData = new Event();
    const formVal = this.mainForm.value;
    eventData.id = formVal.id;
    eventData.name = formVal.name;
    eventData.place = formVal.place;
    eventData.placeUrl = formVal.placeUrl;
    eventData.shortDescription = formVal.shortDescription;
    eventData.fullDescription = formVal.fullDescription;
    eventData.url = formVal.url;
    eventData.from = new Date(this.dateTimeToString(formVal.fromDate, formVal.fromTime));
    eventData. to = new Date(this.dateTimeToString(formVal.toDate, formVal.toTime));

    // Process image.
    let image = this.mainForm.value['image'];
    delete this.mainForm.value['image'];
    if (image) {
      this.imageUploadService.postFile(image['file']).subscribe(data => {
        this.mainForm.patchValue({imageUrl: data.url});
        eventData.imageUrl = this.mainForm.value.imageUrl;

        if (this.editMode) { // FIXME: When cleared, ID will be replaced. Remove clear button altogether?
          this.modifyEvent(eventData);
        }
        else {
          this.planEvent(eventData);
        }
      }, err => {
        if (err.status == HttpStatusCode.Conflict) {
          this.mainForm.patchValue({imageUrl: err.error.url});
          eventData.imageUrl = this.mainForm.value.imageUrl;

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
      this.mainForm.patchValue({imageUrl: null});
      eventData.imageUrl = this.mainForm.value.imageUrl;

      if (this.editMode) { // FIXME: When cleared, ID will be replaced. Remove clear button altogether?
        this.modifyEvent(eventData);
      }
      else {
        this.planEvent(eventData);
      }
    }
  }

  planEvent(eventData: Event) {
    this.eventsService.planEventRequest(eventData).subscribe(
      res => {
        this.mainForm.reset();
        this.eventsService.refreshEventsList();
        this.toastr.success('Akce úspěšně naplánována.', 'Naplánovat akci');
      },
      err => {
        console.log(err);
        this.toastr.error('Naplánování akce selhalo.', 'Naplánovat akci');
      }
    );
  }

  modifyEvent(eventData: Event) {
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
    this.mainForm.patchValue({image: {file: event.target.files.item(0)}});
  }
}
