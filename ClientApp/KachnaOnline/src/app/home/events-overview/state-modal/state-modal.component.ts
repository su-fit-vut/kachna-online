// state-modal.component.ts
// Author: František Nečas

import { Component, Input, OnInit } from '@angular/core';
import { ClubState } from "../../../models/states/club-state.model";
import { ClubStateTypes } from "../../../models/states/club-state-types.model";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: 'app-state-modal',
  templateUrl: './state-modal.component.html',
  styleUrls: ['./state-modal.component.css']
})
export class StateModalComponent implements OnInit {
  @Input() state: ClubState;

  constructor(public activeModal: NgbActiveModal) { }

  public get clubState(): typeof ClubStateTypes {
    return ClubStateTypes;
  }

  ngOnInit(): void {
  }

}
