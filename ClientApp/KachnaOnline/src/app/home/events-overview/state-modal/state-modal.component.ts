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
  heading = "";
  constructor(public activeModal: NgbActiveModal) { }

  public get clubState(): typeof ClubStateTypes {
    return ClubStateTypes;
  }

  ngOnInit(): void {
    switch (this.state.state) {
      case ClubStateTypes.OpenBar:
        this.heading = "Otvíračka s barem";
        break;
      case ClubStateTypes.OpenEvent:
        this.heading = "Veřejná akce";
        break;
      case ClubStateTypes.Private:
        this.heading = "Soukromá akce";
        break;
      case ClubStateTypes.OpenTearoom:
        this.heading = "Čajovna";
        break;
    }
  }

}
