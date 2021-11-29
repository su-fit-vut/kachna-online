import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { FormControl, FormGroup } from "@angular/forms";
import { StatesService } from "../../shared/services/states.service";
import { ClubStateTypes } from "../../models/states/club-state-types.model";
import { NgbCalendar } from "@ng-bootstrap/ng-bootstrap";

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
              private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.planningNew = this.route.snapshot.data.planningNew ?? true;
  }

  onSubmit(): void {

  }

}
