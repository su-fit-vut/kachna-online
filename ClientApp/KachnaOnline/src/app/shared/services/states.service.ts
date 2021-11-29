import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { ToastrService } from "ngx-toastr";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { ClubState } from "../../models/states/club-state.model";
import { ClubStateTypes } from "../../models/states/club-state-types.model";

@Injectable({
  providedIn: 'root'
})
export class StatesService {

  readonly StatesUrl = environment.baseApiUrl + '/states';

  constructor(
    private http: HttpClient,
    private toastrService: ToastrService,
  ) {}

  get(id: number): Observable<ClubState> {
    return this.http.get<ClubState>(`${this.StatesUrl}/${id}`);
  }

  getCurrent(): Observable<ClubState> {
    return this.http.get<ClubState>(`${this.StatesUrl}/current`);
  }

  getNext(type: ClubStateTypes): Observable<ClubState> {
    return this.http.get<ClubState>(`${this.StatesUrl}/next?type=${type}`);
  }

  getMonth(month: Date, peekNextMonth: boolean = true): Observable<ClubState[]> {
    let firstDay = new Date(month.getFullYear(), month.getMonth(), month.getDay(), 0, 0, 0);
    firstDay.setDate(1);
    let lastDay;
    if (peekNextMonth) {
      // 35 is the number of days to show when presenting a calendar
      // firstDay is 00:00:00, so last day is firstDay + 36 days - 1 second (let's not care about leap seconds and DST...)
      lastDay = new Date(firstDay.getTime() + 36 * 86400000 - 1);
    } else {
      lastDay = new Date(month.getFullYear(), month.getMonth() + 1, -1, 23, 59, 59);
    }
    return this.http.get<ClubState[]>(`${this.StatesUrl}?from=${firstDay.toISOString()}&to=${lastDay.toISOString()}`);
  }

}
