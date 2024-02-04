import { Component, OnInit } from '@angular/core';
import { ClubState } from "../../models/states/club-state.model";
import { StatesService } from "../../shared/services/states.service";
import { ClubStateTypes } from "../../models/states/club-state-types.model";
import { catchError } from "rxjs/operators";
import { EMPTY, throwError } from "rxjs";
import { HttpErrorResponse } from "@angular/common/http";

@Component({
  selector: 'app-upcoming-openings',
  templateUrl: './upcoming-openings.component.html',
  styleUrls: ['./upcoming-openings.component.css']
})
export class UpcomingOpeningsComponent implements OnInit {

  constructor(public service: StatesService) { }

  nextChillzone: ClubState;
  nextBar: ClubState;
  nextTearoom: ClubState;

  hasChillzone: boolean;
  hasBar: boolean;
  hasTearoom: boolean;

  ngOnInit(): void {
    this.service.getNext(ClubStateTypes.OpenChillzone)
      .pipe(catchError((error: HttpErrorResponse) => {
        if (error.status == 404) {
          this.hasChillzone = false;
          return EMPTY;
        }
        return throwError(error);
      }))
      .subscribe(e => {
        this.nextChillzone = e;
        this.hasChillzone = true;
      });

    this.service.getNext(ClubStateTypes.OpenBar)
      .pipe(catchError((error: HttpErrorResponse) => {
        if (error.status == 404) {
          this.hasBar = false;
          return EMPTY;
        }
        return throwError(error);
      }))
      .subscribe(e => {
        this.nextBar = e;
        this.hasBar = true;
      });

    this.service.getNext(ClubStateTypes.OpenTearoom)
      .pipe(catchError((error: HttpErrorResponse) => {
        if (error.status == 404) {
          this.hasTearoom = false;
          return EMPTY;
        }
        return throwError(error);
      }))
      .subscribe(e => {
        this.nextTearoom = e;
        this.hasTearoom = true;
      });
  }

}
