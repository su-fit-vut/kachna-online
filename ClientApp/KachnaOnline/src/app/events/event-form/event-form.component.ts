import { Event } from '../../models/events/event.model';
import { EventsService } from '../../shared/services/events.service';
import { Component, Input, OnInit } from '@angular/core';
import {
  NgForm,
  FormBuilder,
  Validators,
  ValidatorFn,
  AbstractControl,
  ValidationErrors
} from "@angular/forms";
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from "@angular/router";
import { NgbCalendar, NgbDate, NgbDateNativeAdapter, NgbDateStruct, NgbTimeStruct } from "@ng-bootstrap/ng-bootstrap";
import { throwError } from "rxjs";
import { HttpStatusCode } from "@angular/common/http";
import { ImageUploadService } from "../../shared/services/image-upload.service";
import { EventModification } from "../../models/events/event-modification.model";
import { DateUtils } from "../../shared/utils/date-utils";


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
    private fb: FormBuilder
  ) { }

  dateRangeValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    let start = this.nativeDateAdapter.toModel(control.get('fromDate')?.value);
    let end = this.nativeDateAdapter.toModel(control.get('toDate')?.value);
    let startTime = control.get('fromTime')?.value;
    let endTime = control.get('toTime')?.value;

    if (!start || !end || !startTime || !endTime) {
      // Should be handled be required validators but make TS happy
      return {datesNotSet: true};
    }

    start.setHours(startTime?.hour, startTime?.minute, 0, 0);
    end.setHours(endTime?.hour, endTime?.minute, 0, 0);

    let now = new Date();
    now.setSeconds(0, 0);

    if (start < now) {
      return {planningForPast: true};
    }

    return end > start ? null : {incorrectDateRange: true};
  }

  urlValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }
    let valid = true;
    try {
      new URL(control.value);
    } catch {
      valid = false;
    }
    return valid ? null : {invalidUrl: true};
  }

  form = this.fb.group({
    id: [-1],
    name: ["", [Validators.required, Validators.maxLength(128)]],
    shortDescription: ["", [Validators.required, Validators.maxLength(512)]],
    fullDescription: [""],
    imageUrl: ["", Validators.maxLength(512)],
    image: this.fb.group({
      file: [undefined]
    }),
    place: ["", Validators.maxLength(256)],
    placeUrl: ["", [Validators.maxLength(512), this.urlValidator]],
    url: ["", [Validators.maxLength(512), this.urlValidator]],
    fromDate: [this.calendar.getToday(), Validators.required],
    fromTime: [{hour: new Date().getHours(), minute: new Date().getMinutes()}, Validators.required],
    toDate: [this.calendar.getToday(), Validators.required],
    toTime: [{hour: new Date().getHours() + 1, minute: new Date().getMinutes()}, Validators.required],
  }, {validators: [this.dateRangeValidator]})

  @Input() editMode: boolean = false;
  jumbotronText: string = "Napl??novat akci";
  submitText: string = "P??idat akci";
  image: string = "";

  ngOnInit(): void {
    if (this.editMode) {
      this.jumbotronText = "Upravit akci";
      this.submitText = "Ulo??it zm??ny";
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
          this.toastr.error("Sta??en?? dat o akci se nezda??ilo.", "Upravit akci");
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
    const to = this.joinDateTime(formVal.toDate, formVal.toTime);
    if (!this.verifyDates(from, to)) {
      return;
    }
    eventData.from = DateUtils.dateTimeToString(formVal.fromDate, formVal.fromTime, this.nativeDateAdapter);
    eventData.to = DateUtils.dateTimeToString(formVal.toDate, formVal.toTime, this.nativeDateAdapter);

    // Process image.
    let image = this.form.value['image'];
    delete this.form.value['image'];
    if (image.file) {
      this.imageUploadService.postFile(image['file']).subscribe(data => {
        this.form.patchValue({imageUrl: data.url});
        eventData.imageUrl = this.form.value.imageUrl;

        if (this.editMode) { // FIXME: When cleared, ID will be replaced. Remove clear button altogether?
          this.modifyEvent(eventData);
        } else {
          this.planEvent(eventData);
        }
      }, err => {
        if (err.status == HttpStatusCode.Conflict) {
          this.form.patchValue({imageUrl: err.error.url});
          eventData.imageUrl = this.form.value.imageUrl;

          if (this.editMode) { // FIXME: When cleared, ID will be replaced. Remove clear button altogether?
            this.modifyEvent(eventData);
          } else {
            this.planEvent(eventData);
          }
        } else {
          this.toastr.error("Nepoda??ilo se nahr??t obr??zek na server.");
        }
      })
    } else {
      this.form.patchValue({imageUrl: null});
      eventData.imageUrl = this.form.value.imageUrl;

      if (this.editMode) { // FIXME: When cleared, ID will be replaced. Remove clear button altogether?
        this.modifyEvent(eventData);
      } else {
        this.planEvent(eventData);
      }
    }
  }

  private joinDateTime(date: NgbDateStruct, time: NgbTimeStruct): Date | null {
    let dateObj = this.nativeDateAdapter.toModel(date);
    dateObj?.setHours(time.hour);
    dateObj?.setMinutes(time.minute);
    dateObj?.setSeconds(0);
    return dateObj;
  }

  private verifyDates(from: Date | null, to: Date | null): boolean {
    if (!from || !to) {
      this.toastr.error("Akce mus?? m??t nastaven?? po????tek i konec akce.", "Pl??nov??n?? akce")
      return false;
    }
    if (from.getTime() < Date.now()) {
      this.toastr.error("Akce nem????e za????nat v minulosti. Upravte term??n po????tku akce.", "Pl??nov??n?? akce")
      return false;
    }
    if (from.getTime() < Date.now()) {
      this.toastr.error("Akce nem????e kon??it v minulosti. Upravte term??n konce akce.", "Pl??nov??n?? akce")
      return false;
    }
    if (from >= to) {
      this.toastr.error("Akce nem????e za????nat po jej??m konci. Upravte term??n po????tku nebo konce akce.", "Pl??nov??n?? akce")
      return false;
    }

    return true;
  }

  planEvent(eventData: EventModification) {
    this.eventsService.planEventRequest(eventData).subscribe(
      res => {
        this.form.reset();
        this.eventsService.refreshEventsList();
        this.toastr.success('Akce ??sp????n?? napl??nov??na.', 'Napl??novat akci');
        this.router.navigate(["/events", res.id]).finally();
      },
      err => {
        console.log(err);
        this.toastr.error('Napl??nov??n?? akce selhalo.', 'Napl??novat akci');
      }
    );
  }

  modifyEvent(eventData: EventModification) {
    let eventId = eventData.id;
    this.eventsService.modifyEventRequest(eventData).subscribe(
      res => {
        this.eventsService.refreshEventsList();
        this.toastr.success('Akce ??sp????n?? upravena.', 'Upravit akci');
        this.router.navigate(["/events", eventId]).then();
      },
      err => {
        console.log(err);
        this.toastr.error('??prava akce selhala.', 'Upravit akci');
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

  imageChanged(event: any): void {
    this.form.patchValue({image: {file: event.target.files.item(0)}});
  }
}
