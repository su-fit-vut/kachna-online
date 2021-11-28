import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { ToastrService } from "ngx-toastr";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { RepeatingState } from "../../models/states/repeating-state.model";
import { map, tap } from "rxjs/operators";
import { Time } from "@angular/common";

@Injectable({
  providedIn: 'root'
})
export class RepeatingStatesService {

  readonly RepeatingStatesUrl = environment.baseApiUrl + '/states/repeating';

  constructor(
    private http: HttpClient,
    private toastrService: ToastrService,
  ) {}

  get(): Observable<RepeatingState[]> {
    return this.http.get<RepeatingState[]>(`${this.RepeatingStatesUrl}/active/now`);
  }

}
