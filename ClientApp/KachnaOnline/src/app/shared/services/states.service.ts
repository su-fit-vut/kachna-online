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

  getCurrent(): Observable<ClubState> {
    return this.http.get<ClubState>(`${this.StatesUrl}/current`);
  }

  getNext(type: ClubStateTypes): Observable<ClubState> {
    return this.http.get<ClubState>(`${this.StatesUrl}/next?type=${type}`);
  }

}
