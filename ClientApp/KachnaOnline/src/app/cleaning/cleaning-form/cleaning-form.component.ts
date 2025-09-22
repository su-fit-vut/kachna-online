import { Cleaning } from '../../models/cleaning/cleaning.model';
import { CleaningModification } from '../../models/cleaning/cleaning-modification.model';
import { CleaningsService } from '../../shared/services/cleanings.service';
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
import { Observable, throwError } from "rxjs";
import { HttpStatusCode } from "@angular/common/http";
import { DateUtils } from "../../shared/utils/date-utils";
import { AuthenticationService } from 'src/app/shared/services/authentication.service';
import { UserDetail } from 'src/app/models/users/user.model';

@Component({
  selector: 'app-cleaning-form',
  templateUrl: './cleaning-form.component.html',
  styleUrls: ['./cleaning-form.component.css']
})
export class CleaningFormComponent implements OnInit {
  showOtherPlace: Boolean = false;
  placeInstructions: {[key: string]: string};

  constructor(
    public cleaningsService: CleaningsService,
    public calendar: NgbCalendar,
    private nativeDateAdapter: NgbDateNativeAdapter,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    public authenticationService: AuthenticationService,
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

    return end > start ? null : {incorrectDateRange: true};
  }

  form = this.fb.group({
    id: [-1],
    name: ["", [Validators.required, Validators.maxLength(128)]],
    cleaningInstructions: [""],
    place: ["", Validators.maxLength(256)],
    assignedUsersIds: [[]],
    idealParticipantsCount: [3, [Validators.required, Validators.min(1), Validators.max(100)]],
    idsToUsername: [[]],
    fromDate: [this.calendar.getToday(), Validators.required],
    fromTime: [{hour: new Date().getHours(), minute: new Date().getMinutes()}, Validators.required],
    toDate: [this.calendar.getToday(), Validators.required],
    toTime: [{hour: new Date().getHours() + 1, minute: new Date().getMinutes()}, Validators.required],
    finished: [false]
  }, {validators: [this.dateRangeValidator]});


  @Input() editMode: boolean = false;
  jumbotronText: string = "Naplánovat úklid";
  submitText: string = "Přidat úklid";

  private prepareInstructions(cleaningModels: Cleaning[]) {
    for(let cleaning of cleaningModels) {
      this.placeInstructions[cleaning.place] = cleaning.cleaningInstructions;
    }
  }

  ngOnInit(): void {
    if (this.editMode) {
      this.jumbotronText = "Upravit úklid";
      this.submitText = "Uložit změny";
      this.route.paramMap.subscribe(params => {
        let cleaningId = Number(params.get('cleaningId'));
        this.cleaningsService.getCleaningRequest(cleaningId).toPromise()
          .then((edittedCleaning: Cleaning) => {
            this.form.controls.id.setValue(edittedCleaning.id);
            this.form.controls.name.setValue(edittedCleaning.name);
            this.form.controls.cleaningInstructions.setValue(edittedCleaning.cleaningInstructions);
            this.form.controls.place.setValue(edittedCleaning.place);
            this.form.controls.assignedUsersIds.setValue(edittedCleaning.assignedUsersIds);
            this.form.controls.idsToUsername.setValue(edittedCleaning.idsToUsername);
            this.form.controls.idealParticipantsCount.setValue(edittedCleaning.idealParticipantsCount);
            this.form.controls.fromDate.setValue(this.nativeDateAdapter.fromModel(edittedCleaning.from));
            this.form.controls.fromTime.setValue({
              hour: edittedCleaning.from.getHours(),
              minute: edittedCleaning.from.getMinutes(),
            });
            this.form.controls.toDate.setValue(this.nativeDateAdapter.fromModel(edittedCleaning.to));
            this.form.controls.toTime.setValue({
              hour: edittedCleaning.to.getHours(),
              minute: edittedCleaning.to.getMinutes(),
            });
            this.form.controls.finished.setValue(edittedCleaning.finished);
          }).catch(err => {
          this.toastr.error("Stažení dat o úklidu se nezdařilo.", "Upravit úklid");
          return throwError(err);
        });
      });      
    } else {
      this.cleaningsService.cleaningDetail = new Cleaning();

      this.cleaningsService.getYearCleanings(new Date).subscribe(
        (res: Cleaning[]) => {
          this.prepareInstructions(res);
        }
      );
    }
  }

  ngOnChanges() {
  }

  onSubmit() {
    let cleaningData = new CleaningModification();
    const formVal = this.form.value;

    cleaningData.id = formVal.id;
    cleaningData.name = formVal.name;
    cleaningData.place = formVal.place;
    cleaningData.cleaningInstructions = formVal.cleaningInstructions;
    cleaningData.assignedUsersIds = formVal.assignedUsersIds;
    cleaningData.idealParticipantsCount = formVal.idealParticipantsCount;
    cleaningData.finished = formVal.finished;

    //other has to be taken from text box
    if (cleaningData.place === 'other') {
      cleaningData.place = formVal.place;
    }

    // Process date and time values.
    const from = this.joinDateTime(formVal.fromDate, formVal.fromTime);
    const to = this.joinDateTime(formVal.toDate, formVal.toTime);
    if (!this.verifyDates(from, to)) {
      return;
    }
    cleaningData.from = DateUtils.dateTimeToString(formVal.fromDate, formVal.fromTime, this.nativeDateAdapter);
    cleaningData.to = DateUtils.dateTimeToString(formVal.toDate, formVal.toTime, this.nativeDateAdapter);

    if (this.editMode) {
      this.modifyCleaning(cleaningData);
    } else {
      this.planCleaning(cleaningData);
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
      this.toastr.error("Úklid musí mít nastavený počátek i konec.", "Plánování úklidu")
      return false;
    }
    if (from >= to) {
      this.toastr.error("Úklid nemůže začínat po jeho konci. Upravte termín počátku nebo konce úklidu.", "Plánování úklidu")
      return false;
    }

    return true;
  }

  planCleaning(cleaningData: CleaningModification) {
    this.cleaningsService.planCleaningRequest(cleaningData).subscribe(
      res => {
        this.form.reset();
        this.cleaningsService.refreshCleaningsList();
        this.toastr.success('Úklid úspěšně naplánován.', 'Naplánovat úklid');
        this.router.navigate(["/cleanings", res.id]).finally();
      },
      err => {
        console.log(err);
        this.toastr.error('Naplánování úklidu selhalo.', 'Naplánovat úklid');
      }
    );
  }

  modifyCleaning(cleaningData: CleaningModification) {
    let cleaningId = cleaningData.id;
    this.cleaningsService.modifyCleaningRequest(cleaningData).subscribe(
      res => {
        this.cleaningsService.refreshCleaningsList();
        this.toastr.success('Úklid úspěšně upraven.', 'Upravit úklid');
        this.router.navigate(["/cleanings", cleaningId]).then();
      },
      err => {
        console.log(err);
        this.toastr.error('Úprava úklidu selhala.', 'Upravit úklid');
      }
    );
  }

  clearForm(form: NgForm) {
    form.form.reset();
    this.cleaningsService.cleaningDetail = new Cleaning();
  }

  onPlaceChange(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this.showOtherPlace = value === 'other';
  
    if (!this.showOtherPlace) {
      this.form.get('otherPlace')?.reset();
    }

    // Auto-set cleaningInstructions depending on the place
    switch (value) {
      case 'park':
        this.form.get('cleaningInstructions')?.setValue('Sbírejte odpadky v parku a roztřiďte plasty, sklo a papír.');
        break;
      case 'school':
        this.form.get('cleaningInstructions')?.setValue('Zaměřte se na školní dvůr, hřiště a okolní chodníky.');
        break;
      case 'street':
        this.form.get('cleaningInstructions')?.setValue('Ukliďte chodníky, zastávky a veřejné koše.');
        break;
      case 'other':
        this.form.get('cleaningInstructions')?.reset(); // let user type freely
        break;
      default:
        this.form.get('cleaningInstructions')?.reset();
    }
  }

  removeAssignedUser(userId: number) {
    const assignedUsers = this.form.controls.assignedUsersIds.value as number[];
    const idsToUsername = this.form.controls.idsToUsername.value as {item1: number, item2: string}[];
    const indexAss = assignedUsers.indexOf(userId);
    const indexId = idsToUsername.findIndex(obj => obj.item1 == userId);
    if(indexAss == -1) {
      this.toastr.error('Uživatel byl již smazán.', 'Upravit přiřazené uživatele');
      return;
    }
    assignedUsers.splice(indexAss, 1); // remove from array
    this.form.controls.assignedUsersIds.setValue([...assignedUsers]); // trigger change
    idsToUsername.splice(indexId, 1);
    this.form.controls.idsToUsername.setValue([...idsToUsername]);
  }
}
