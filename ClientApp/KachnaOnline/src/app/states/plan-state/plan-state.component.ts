import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import {
  AbstractControl,
  FormBuilder,
  ValidationErrors,
  ValidatorFn,
  Validators
} from "@angular/forms";
import { StatesService } from "../../shared/services/states.service";
import { ClubStateTypes } from "../../models/states/club-state-types.model";
import { NgbCalendar, NgbDateNativeAdapter, NgbDateStruct, NgbTimeStruct } from "@ng-bootstrap/ng-bootstrap";
import { StateModification } from "../../models/states/state-modification.model";
import { ClubState } from "../../models/states/club-state.model";
import { DateUtils } from "../../shared/utils/date-utils";
import { Event } from "../../models/events/event.model";
import { EventsService } from "../../shared/services/events.service";

enum Mode {
  ModifyCurrent,
  ModifyPlanned,
  CreateCurrent,
  CreatePlanned
}

@Component({
  selector: 'app-plan-state',
  templateUrl: './plan-state.component.html',
  styleUrls: ['./plan-state.component.css']
})
export class PlanStateComponent implements OnInit {
  dateRangeValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    let end = this.nativeDateAdapter.toModel(control.get('plannedEndDate')?.value);
    let endTime = control.get('plannedEndTime')?.value;
    if (!end || !endTime) {
      return {endNotSet: true};
    }

    let now = new Date();
    now.setSeconds(0, 0);

    end.setHours(endTime?.hour, endTime?.minute, 0);
    if (this.mode == Mode.CreateCurrent || this.mode == Mode.ModifyCurrent) {
      // Does not require start date, only check that it ends in the future.
      return end > now ? null : {endInPast: true};
    } else {
      let start = this.nativeDateAdapter.toModel(control.get('startDate')?.value);
      let startTime = control.get('startTime')?.value;
      if (!start || !startTime) {
        return {startNotSet: true};
      }

      start.setHours(startTime?.hour, startTime?.minute, 0, 0);
      if (start < now) {
        return {startInPast: true};
      }

      return start < end ? null : {invalidDateRange: true};
    }
  }

  ST = ClubStateTypes;
  M = Mode;

  mainForm = this.fb.group({
    stateType: [ClubStateTypes.OpenChillzone, Validators.required],
    startDate: [this.calendar.getToday()],
    startTime: [{hour: new Date().getHours(), minute: new Date().getMinutes()}],
    plannedEndDate: [this.calendar.getToday(), Validators.required],
    plannedEndTime: [{hour: new Date().getHours() + 1, minute: new Date().getMinutes()}, Validators.required],
    noteInternal: ['', Validators.maxLength(1024)],
    notePublic: ['', Validators.maxLength(1024)],
  }, {validators: [this.dateRangeValidator]})

  editingId: number;
  mode: Mode;
  referenceState: ClubState;

  navigationEvent: Event;

  constructor(private stateService: StatesService,
              private eventsService: EventsService,
              private calendar: NgbCalendar,
              private nativeDateAdapter: NgbDateNativeAdapter,
              private route: ActivatedRoute,
              private router: Router,
              private fb: FormBuilder) {
    this.navigationEvent = this.router.getCurrentNavigation()?.extras?.state?.event;
  }

  patchForm(start: Date, end: Date) {
    this.mainForm.patchValue({
      startDate: this.nativeDateAdapter.fromModel(start),
      startTime: {
        hour: start.getHours(),
        minute: start.getMinutes()
      },

      plannedEndDate: this.nativeDateAdapter.fromModel(end),
      plannedEndTime: {
        hour: end.getHours(),
        minute: end.getMinutes()
      }
    });
  }

  ngOnInit(): void {
    const stateFetched = (state: ClubState) => {
      this.referenceState = state;

      if (this.mode == Mode.ModifyCurrent && state.state == ClubStateTypes.Closed) {
        this.mode = Mode.CreateCurrent;
      }

      if (this.mode != Mode.CreateCurrent && this.mode != Mode.CreatePlanned && state.state != ClubStateTypes.Closed) {
        this.patchForm(state.start, state.plannedEnd);
        this.mainForm.patchValue({
          noteInternal: state.noteInternal,
          notePublic: state.note
        });
      }

      if (this.mode == Mode.CreatePlanned) {
        if (this.navigationEvent) {
          this.patchForm(this.navigationEvent.from, this.navigationEvent.to);
          this.mainForm.controls.notePublic.setValue("Otevřeno v rámci akce " + this.navigationEvent.name);
        }
      }

      if (this.mode == Mode.ModifyPlanned) {
        this.stateService.getCurrent().subscribe(currentState => {
          if (currentState.id == state.id) {
            this.mode = Mode.ModifyCurrent;
          }
        });
      }
    };

    if (this.route.snapshot.params.id) {
      this.mode = Mode.ModifyPlanned;
      this.editingId = this.route.snapshot.params.id;

      this.stateService.get(this.editingId).subscribe(stateFetched, _ => {
        this.router.navigate(["/states/history"]).finally();
      });
    } else {
      this.mode = (this.route.snapshot.data.planningNew ?? true) ? Mode.CreatePlanned : Mode.ModifyCurrent;
      this.stateService.getCurrent().subscribe(stateFetched);
    }
  }

  closeOrDeleteState(): void {
    if (this.mode == Mode.ModifyPlanned) {
      this.stateService.delete(this.editingId).subscribe(_ => {
        this.router.navigate(["/states/history"]).finally();
      });
    } else {
      this.stateService.closeCurrent().subscribe(_ => {
        this.router.navigate(["/"]).finally();
      });
    }
  }

  onSubmit(): void {
    let dto = new StateModification();
    const val = this.mainForm.value;

    dto.noteInternal = (this.mainForm.controls.noteInternal.dirty && val.noteInternal?.length > 0)
      ? val.noteInternal : null;
    dto.notePublic = (this.mainForm.controls.notePublic.dirty && val.notePublic?.length > 0)
      ? val.notePublic : null;

    let startDate: NgbDateStruct = val.startDate;
    let startTime: NgbTimeStruct = val.startTime;

    let endDate: NgbDateStruct = val.plannedEndDate;
    let endTime: NgbTimeStruct = val.plannedEndTime;

    if (this.mode == Mode.CreateCurrent || this.mode == Mode.CreatePlanned) {
      // Calling a *Plan* endpoint

      if (this.mode == Mode.CreateCurrent) {
        startDate = this.calendar.getToday();
        startTime = {hour: new Date().getHours(), minute: new Date().getMinutes(), second: 0};
      }

      dto.start = DateUtils.dateTimeToString(startDate, startTime, this.nativeDateAdapter);
      dto.plannedEnd = DateUtils.dateTimeToString(endDate, endTime, this.nativeDateAdapter);
      dto.state = val.stateType;

      this.stateService.planNew(dto).subscribe(res => {
        this.linkEvent(res.newState.id);
        this.done();
      }); // TODO
    } else {
      // Calling a *Modify* endpoint

      if (this.mode == Mode.ModifyPlanned) {
        dto.start = DateUtils.dateTimeToString(startDate, startTime, this.nativeDateAdapter);
      } else {
        // Modifying the current state

        delete dto.state;
      }

      dto.plannedEnd = DateUtils.dateTimeToString(endDate, endTime, this.nativeDateAdapter);
      dto.madeById = null; // TODO

      if (this.editingId != null) {
        this.stateService.modify(this.editingId, dto).subscribe(res => this.done()); // TODO
      } else {
        this.stateService.modifyCurrent(dto).subscribe(res => this.done()); // TODO
      }
    }
  }

  done() {
    this.mainForm.reset();

    if (this.mode == Mode.ModifyPlanned || this.mode == Mode.CreatePlanned) {
      this.router.navigate(["/states/history"]).finally();
    } else {
      this.router.navigate(["/"]).finally();
    }
  }

  linkEvent(stateId: number) {
    if (this.navigationEvent) {
      this.eventsService.linkConflictingState(stateId);
    }
  }
}
