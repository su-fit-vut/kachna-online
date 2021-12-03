import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { FormControl, FormGroup } from "@angular/forms";
import { StatesService } from "../../shared/services/states.service";
import { ClubStateTypes } from "../../models/states/club-state-types.model";
import { NgbCalendar, NgbDateNativeAdapter, NgbDateStruct, NgbTimeStruct } from "@ng-bootstrap/ng-bootstrap";
import { StateModification } from "../../models/states/state-modification.model";
import { ClubState } from "../../models/states/club-state.model";
import { DateUtils } from "../../shared/utils/date-utils";

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

  ST = ClubStateTypes;
  M = Mode;

  mainForm = new FormGroup({
    stateType: new FormControl(ClubStateTypes.OpenChillzone),
    startDate: new FormControl(this.calendar.getToday()),
    startTime: new FormControl({hour: new Date().getHours(), minute: new Date().getMinutes()}),
    plannedEndDate: new FormControl(this.calendar.getToday()),
    plannedEndTime: new FormControl({hour: new Date().getHours() + 1, minute: new Date().getMinutes()}),
    noteInternal: new FormControl(''),
    notePublic: new FormControl('')
  });

  editingId: number;
  mode: Mode;
  referenceState: ClubState;

  constructor(public stateService: StatesService,
              public calendar: NgbCalendar,
              private nativeDateAdapter: NgbDateNativeAdapter,
              private route: ActivatedRoute,
              private router: Router) {
  }

  ngOnInit(): void {
    const stateFetched = (state: ClubState) => {
      this.referenceState = state;

      if (this.mode == Mode.ModifyCurrent && state.state == ClubStateTypes.Closed) {
        this.mode = Mode.CreateCurrent;
      }

      if (this.mode != Mode.CreateCurrent && this.mode != Mode.CreatePlanned && state.state != ClubStateTypes.Closed) {
        // TODO: Is there a better, more Angular-ish way to do this mapping?

        this.mainForm.controls.startDate.setValue(this.nativeDateAdapter.fromModel(state.start));
        this.mainForm.controls.startTime.setValue({
          hour: state.start.getHours(),
          minute: state.start.getMinutes()
        });

        this.mainForm.controls.plannedEndDate.setValue(this.nativeDateAdapter.fromModel(state.plannedEnd));
        this.mainForm.controls.plannedEndTime.setValue({
          hour: state.plannedEnd.getHours(),
          minute: state.plannedEnd.getMinutes()
        });

        this.mainForm.controls.noteInternal.setValue(state.noteInternal);
        this.mainForm.controls.notePublic.setValue(state.note);
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

      this.stateService.planNew(dto).subscribe(_ => this.done()); // TODO
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
        this.stateService.modify(this.editingId, dto).subscribe(_ => this.done()); // TODO
      } else {
        this.stateService.modifyCurrent(dto).subscribe(_ => this.done()); // TODO
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
}
