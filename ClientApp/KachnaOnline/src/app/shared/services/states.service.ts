import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { ToastrService } from "ngx-toastr";
import { Observable, of } from "rxjs";
import { environment } from "../../../environments/environment";
import { ClubState } from "../../models/states/club-state.model";
import { ClubStateTypes } from "../../models/states/club-state-types.model";
import { catchError, tap } from "rxjs/operators";
import { StateModification } from "../../models/states/state-modification.model";

@Injectable({
  providedIn: 'root'
})
export class StatesService {

  readonly StatesUrl = environment.baseApiUrl + '/states';

  currentState: ClubState;

  constructor(
    private http: HttpClient,
    private toastrService: ToastrService,
  ) {}

  get(id: number): Observable<ClubState> {
    return this.http.get<ClubState>(`${this.StatesUrl}/${id}`);
  }

  getCurrent(): Observable<ClubState> {
    return this.http.get<ClubState>(`${this.StatesUrl}/current`).pipe(tap(res => this.currentState = res));
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
      lastDay = new Date(month.getFullYear(), month.getMonth() + 1, 0, 23, 59, 59);
    }

    firstDay.setTime(firstDay.getTime() - firstDay.getTimezoneOffset() * 60000);
    lastDay.setTime(lastDay.getTime() - lastDay.getTimezoneOffset() * 60000);
    return this.http.get<ClubState[]>(`${this.StatesUrl}?from=${firstDay.toISOString()}&to=${lastDay.toISOString()}`);
  }

  closeCurrent(): Observable<any> {
    return this.http.delete(`${this.StatesUrl}/current`).pipe(tap(_ => this.getCurrent()));
  }

  planNew(newState: StateModification): Observable<any> {
    delete newState.madeById;
    return this.http.post(`${this.StatesUrl}`, newState)
      .pipe(catchError((err: HttpErrorResponse, _) => this.handlePostError(err)),
        tap(_ => this.toastrService.success("Stav byl naplánován.", "Změna stavu klubu")));
  }

  modifyCurrent(modification: StateModification): Observable<any> {
    delete modification.state;
    delete modification.start;

    return this.http.patch(`${this.StatesUrl}/current`, modification)
      .pipe(catchError((err: HttpErrorResponse, _) => this.handlePatchError(err, true)),
        tap(_ => this.toastrService.success("Aktuální stav byl upraven.", "Změna stavu klubu")));
  }

  private handlePostError(err: HttpErrorResponse): Observable<any> {
    if (err.status === 0) {
      this.toastrService.error("Nepodařilo se odeslat požadavek.", "Změna stavu klubu");
    } else if (err.status === 400) {
      this.toastrService.error("Neplatné nastavení stavu.", "Změna stavu klubu");
    } else if (err.status === 404) {
      this.toastrService.error("Žádný navazující stav není naplánovaný.", "Změna stavu klubu");
    } else if (err.status === 409) {
      this.toastrService.error("Změněný stav by zasahoval do už existujícího naplánovaného stavu.", "Změna stavu klubu");
    }

    // TODO: return something that gives more information about what happened to the component
    return of();
  }

  private handlePatchError(err: HttpErrorResponse, current: boolean): Observable<any> {
    if (err.status === 0) {
      this.toastrService.error("Nepodařilo se odeslat požadavek.", "Změna stavu klubu");
    } else if (err.status === 400) {
      this.toastrService.error("Neplatná změna stavu.", "Změna stavu klubu");
    } else if (err.status === 403) {
      this.toastrService.error(
        "Pro změnu stavu musíte být administrátor stavů. Pro změnu uživatele, který stav vytvořil, musíte být administrátor.",
        "Změna stavu klubu");
    } else if (err.status === 404) {
      this.toastrService.error(current ? "Kachna je aktuálně zavřená, naplánujte nový stav."
        : "Cílový stav neexistuje.", "Změna stavu klubu");
    } else if (err.status === 409) {
      this.toastrService.error("Změněný stav by zasahoval do už existujícího naplánovaného stavu.", "Změna stavu klubu");
    } else if (err.status === 422) {
      this.toastrService.error("Cílový uživatel neexistuje.", "Změna stavu klubu");
    }

    // TODO: return something that gives more information about what happened to the component
    return of();
  }
}
