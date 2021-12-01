import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";
import { FormControl, FormGroup } from "@angular/forms";
import { StatesService } from "../../shared/services/states.service";
import { ClubStateTypes } from "../../models/states/club-state-types.model";
import { NgbCalendar, NgbDateNativeAdapter, NgbDateStruct, NgbTimeStruct } from "@ng-bootstrap/ng-bootstrap";
import { StateModification } from "../../models/states/state-modification.model";

@Component({
  selector: 'app-plan-state',
  templateUrl: './plan-state.component.html',
  styleUrls: ['./plan-state.component.css']
})
export class PlanStateComponent implements OnInit {

  ST = ClubStateTypes;

  mainForm = new FormGroup({
    stateType: new FormControl(ClubStateTypes.OpenChillzone),
    startDate: new FormControl(this.calendar.getToday()),
    startTime: new FormControl({hour: new Date().getHours(), minute: new Date().getMinutes()}),
    plannedEndDate: new FormControl(this.calendar.getToday()),
    plannedEndTime: new FormControl({hour: new Date().getHours() + 1, minute: new Date().getMinutes()}),
    noteInternal: new FormControl(''),
    notePublic: new FormControl('')
  });

  planningNew: boolean = false;

  constructor(public stateService: StatesService,
              public calendar: NgbCalendar,
              private nativeDateAdapter: NgbDateNativeAdapter,
              private route: ActivatedRoute,
              private router: Router) { }

  ngOnInit(): void {
    this.planningNew = this.route.snapshot.data.planningNew ?? true;
    this.stateService.getCurrent().subscribe(currentState => {
      if (!this.planningNew) {
        this.mainForm.controls.plannedEndDate.setValue(this.nativeDateAdapter.fromModel(currentState.plannedEnd));
        this.mainForm.controls.plannedEndTime.setValue({
          hour: currentState.plannedEnd.getHours(),
          minute: currentState.plannedEnd.getMinutes()
        });
        this.mainForm.controls.noteInternal.setValue(currentState.noteInternal);
        this.mainForm.controls.notePublic.setValue(currentState.note);
      }
    });
  }

  closeCurrentState(): void {
    this.stateService.closeCurrent().subscribe(_ => {
      this.router.navigate(["/"]).finally();
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

  onSubmit(): void {
    let dto = new StateModification();
    const val = this.mainForm.value;

    dto.noteInternal = val.noteInternal?.length > 0 ? val.noteInternal : null;
    dto.notePublic = val.notePublic?.length > 0 ? val.notePublic : null;

    if (this.planningNew || this.stateService.currentState == undefined
      || this.stateService.currentState.state == ClubStateTypes.Closed
      || val.stateType != this.stateService.currentState.state) {

      let startDate: NgbDateStruct = val.startDate;
      let startTime: NgbTimeStruct = val.startTime;
      let endDate: NgbDateStruct = val.plannedEndDate;
      let endTime: NgbTimeStruct = val.plannedEndTime;

      if (!this.planningNew) {
        startDate = this.calendar.getToday();
        startTime = {hour: new Date().getHours(), minute: new Date().getMinutes(), second: 0};
      }

      dto.start = this.dateTimeToString(startDate, startTime);
      dto.plannedEnd = this.dateTimeToString(endDate, endTime);
      dto.state = val.stateType;

      this.stateService.planNew(dto).subscribe(_ => this.done()); // TODO
    } else {
      let endDate: NgbDateStruct = val.plannedEndDate;
      let endTime: NgbTimeStruct = val.plannedEndTime;

      dto.plannedEnd = this.dateTimeToString(endDate, endTime);
      dto.madeById = null; // TODO

      this.stateService.modifyCurrent(dto).subscribe(_ => this.done()); // TODO
    }
  }

  done() {
    this.mainForm.reset();
    this.router.navigate(["/"]).finally();
  }
}
