import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from "@angular/common/http";
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
  ) {
  }

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
    let firstDay = new Date(month.getFullYear(), month.getMonth(), month.getDate(), 0, 0, 0);
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

  getBetween(start: Date, end: Date): Observable<ClubState[]> {
    let params = new HttpParams();
    params = params.set('from', start.toISOString());
    params = params.set('to', end.toISOString());
    return this.http.get<ClubState[]>(`${this.StatesUrl}`, {params: params});
  }

  closeCurrent(): Observable<any> {
    return this.http.delete(`${this.StatesUrl}/current`).pipe(
      catchError((err: HttpErrorResponse, x) => this.handleDeleteError(err, true, x)),
      tap(_ => this.getCurrent()));
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.StatesUrl}/${id}`).pipe(
      catchError((err: HttpErrorResponse, x) => this.handleDeleteError(err, false, x)),
      tap(_ => this.getCurrent()),
      tap(_ => this.toastrService.success("Napl??novan?? stav byl odstran??n.", "Odstran??n?? stavu")));
  }

  planNew(newState: StateModification): Observable<any> {
    delete newState.madeById;
    return this.http.post(`${this.StatesUrl}`, newState)
      .pipe(catchError((err: HttpErrorResponse, x) => this.handlePostError(err, x)),
        tap(_ => this.toastrService.success("Stav byl napl??nov??n.", "Zm??na stavu klubu")));
  }

  modifyCurrent(modification: StateModification): Observable<any> {
    delete modification.state;
    delete modification.start;

    return this.http.patch(`${this.StatesUrl}/current`, modification)
      .pipe(catchError((err: HttpErrorResponse, x) => this.handlePatchError(err, true, x)),
        tap(_ => this.toastrService.success("Aktu??ln?? stav byl upraven.", "Zm??na stavu klubu")));
  }

  modify(id: number, modification: StateModification): Observable<any> {
    delete modification.state;

    return this.http.patch(`${this.StatesUrl}/${id}`, modification)
      .pipe(catchError((err: HttpErrorResponse, x) => this.handlePatchError(err, true, x)),
        tap(_ => this.toastrService.success("Stav byl upraven.", "Zm??na stavu klubu")));
  }

  private handleDeleteError(err: HttpErrorResponse, current: boolean, x: Observable<any>): Observable<any> {
    const title = current ? "Ukon??en?? stavu" : "Odstran??n?? stavu";

    if (err.status === 0) {
      this.toastrService.error("Nepoda??ilo se odeslat po??adavek.", title);
    } else if (err.status === 404) {
      this.toastrService.error(current ? "Kachna je u?? zav??en??." : "C??lov?? stav neexistuje.", title);
    } else if (err.status == 409) {
      this.toastrService.error("C??lov?? stav u?? za??al, nen?? mo??n?? jej odstranit.", title);
    }

    return of();
  }

  private handlePostError(err: HttpErrorResponse, x: Observable<any>): Observable<any> {
    if (err.status === 0) {
      this.toastrService.error("Nepoda??ilo se odeslat po??adavek.", "Zm??na stavu klubu");
    } else if (err.status === 400) {
      this.toastrService.error("Neplatn?? nastaven?? stavu.", "Zm??na stavu klubu");
    } else if (err.status === 404) {
      this.toastrService.error("????dn?? navazuj??c?? stav nen?? napl??novan??.", "Zm??na stavu klubu");
    } else if (err.status === 409) {
      this.toastrService.error("Zm??n??n?? stav by zasahoval do u?? existuj??c??ho napl??novan??ho stavu.", "Zm??na stavu klubu");
    }

    return of();
  }

  private handlePatchError(err: HttpErrorResponse, current: boolean, x: Observable<any>): Observable<any> {
    if (err.status === 0) {
      this.toastrService.error("Nepoda??ilo se odeslat po??adavek.", "Zm??na stavu klubu");
    } else if (err.status === 400) {
      this.toastrService.error("Neplatn?? zm??na stavu.", "Zm??na stavu klubu");
    } else if (err.status === 403) {
      this.toastrService.error(
        "Pro zm??nu stavu mus??te b??t administr??tor stav??. Pro zm??nu u??ivatele, kter?? stav vytvo??il, mus??te b??t administr??tor.",
        "Zm??na stavu klubu");
    } else if (err.status === 404) {
      this.toastrService.error(current ? "Kachna je aktu??ln?? zav??en??, napl??nujte nov?? stav."
        : "C??lov?? stav neexistuje.", "Zm??na stavu klubu");
    } else if (err.status === 409) {
      this.toastrService.error("Zm??n??n?? stav by zasahoval do u?? existuj??c??ho napl??novan??ho stavu.", "Zm??na stavu klubu");
    } else if (err.status === 422) {
      this.toastrService.error("C??lov?? u??ivatel neexistuje.", "Zm??na stavu klubu");
    }

    return of();
  }
}
