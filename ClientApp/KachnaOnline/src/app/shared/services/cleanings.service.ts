import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Cleaning } from "../../models/cleaning/cleaning.model";
import { formatDate } from "@angular/common";
import { CleaningModification } from "../../models/cleaning/cleaning-modification.model";
import { DeletionConfirmationModalComponent, DeletionType,
} from "../components/deletion-confirmation-modal/deletion-confirmation-modal.component";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { StatesService } from "./states.service";
import { CleaningFinishConfirmationModalComponent } from '../components/cleaning-finish-confirmation-modal/cleaning-finish-confirmation-modal.component';

@Injectable({
  providedIn: 'root'
})
export class CleaningsService {
  readonly CleaningsUrl = environment.baseApiUrl + '/cleanings';
  readonly StatesUrl = environment.baseApiUrl + '/states';

  constructor(
    private http: HttpClient,
    private toastr: ToastrService,
    private _modalService: NgbModal,
  ) {
  }

  cleaningDetail: Cleaning = new Cleaning();
  cleaningsList: Cleaning[] = [];
  // conflictingStatesList: ClubState[] = [];
  // shownConflictingStatesList: ClubState[] = [];
  unlinkedOnly: boolean;

  cleaningsBackRoute: string = "..";

  getNextPlannedCleaning(): Observable<Cleaning[]> {
      return this.http.get<Cleaning[]>(`${this.CleaningsUrl}/next`);
  }

  getCleaningsInInterval(from?: string, to?: string): Observable<Cleaning[]> {
    let params = new HttpParams();
    if (from) {
      params = params.set("from", from);
    }
    if (to) {
      params = params.set("to", to);
    }

    return this.http.get<Cleaning[]>(this.CleaningsUrl, {params: params});
  }

  getMonthCleanings(month: Date, peekNextMonth: boolean = true): Observable<Cleaning[]> {
    const year = month.getUTCFullYear();
    const monthNum = month.getUTCMonth();

    let firstDay = new Date(Date.UTC(year, monthNum, 1, 0, 0, 0));
    let lastDay;

    if (peekNextMonth) {
      // 35 is the number of days to show when presenting a calendar
      // firstDay is 00:00:00, so last day is firstDay + 36 days - 1 second (let's not care about leap seconds and DST...)
      lastDay = new Date(firstDay.getTime() + 36 * 86400000 - 1);
    } else {
      lastDay = new Date(Date.UTC(year, monthNum + 1, 1, 0, 0, 0));
    }

    return this.getBetween(firstDay, lastDay);
  }

  getYearCleanings(year: Date) {
    let firstDayOfYear = new Date(year.getFullYear(), 0, 1, 0, 0, 0, 0);
    let lastDayOfYear = new Date(year.getFullYear(), 11, 31, 23, 59, 59);

    return this.getBetween(firstDayOfYear, lastDayOfYear);
  }

  getBetween(start: Date, end: Date): Observable<Cleaning[]> {
    return this.getCleaningsInInterval(start.toISOString(), end.toISOString());
  }

  getCleaningRequest(cleaningId: number): Observable<Cleaning> {
    return this.http.get<Cleaning>(`${this.CleaningsUrl}/${cleaningId}`, {});
  }

  getCurrentCleaningsRequest(): Observable<Cleaning[]> {
    return this.http.get<Cleaning[]>(`${this.CleaningsUrl}/current`);
  }

  getUnfinishedCleaningsRequest(): Observable<Cleaning[]> {
    return this.http.get<Cleaning[]>(`${this.CleaningsUrl}/unfinished`);
  }

  refreshCurrentCleanings() {
    this.getCurrentCleaningsRequest().toPromise()
      .then((res: Cleaning[]) => {
        this.cleaningsList = res;
      }).catch((error: any) => {
      console.log(error);
      this.toastr.error("Nepodařilo se načíst úklidy.", "Úklidy");
      return;
    });
  }

  planCleaningRequest(cleaningData: CleaningModification): Observable<any> {
    return this.http.post(this.CleaningsUrl, cleaningData);
  }

  modifyCleaningRequest(cleaningData: CleaningModification): Observable<any> {
    return this.http.put(`${this.CleaningsUrl}/${this.cleaningDetail.id}`, cleaningData);
  }

  removeCleaningRequest(cleaningId: number): Observable<any> {
    return this.http.delete(`${this.CleaningsUrl}/${cleaningId}`);
  }

  finishCleaningRequest(cleaningId: number): Observable<any> {
    return this.http.post(`${this.CleaningsUrl}/finish/${cleaningId}`, null);
  }

  joinCleaningRequest(cleaningId: number): Observable<any> {
    return this.http.post(`${this.CleaningsUrl}/join/${cleaningId}`, null);
  }

  leaveCleaningRequest(cleaningId: number): Observable<any> {
    return this.http.post(`${this.CleaningsUrl}/leave/${cleaningId}`, null);
  }

  refreshCleaningsList() {
    this.getCleaningsInInterval().toPromise()
      .then((res: Cleaning[]) => {
        this.cleaningsList = res;
      }).catch((error: any) => {
        console.log(error);
        this.toastr.error("Nepodařilo se načíst úklidy.", "Úklidy");
        return;
      });
  }

  populateForm(selectedCleaningDetail: Cleaning) {
    this.cleaningDetail = Object.assign({}, selectedCleaningDetail);
  }

  private handleFinishCleaning(cleaningDetail?: Cleaning) {
    if (cleaningDetail) {
      this.cleaningDetail = Object.assign({}, cleaningDetail);
    }

    this.finishCleaningRequest(this.cleaningDetail.id).subscribe(
      res => {
        this.refreshCleaningsList();
        this.toastr.success("Úklid úspěšně dokončen.", "Dokončení úklidu");
      },
      err => {
        console.log(err)
        this.toastr.error("Úklid se nepovedlo dokončit.", "Dokončení úklidu");
      }
    )
  }

  handleRemoveCleaning(cleaningDetail?: Cleaning) {
    if (cleaningDetail) {
      this.cleaningDetail = Object.assign({}, cleaningDetail);
    }

    this.removeCleaningRequest(this.cleaningDetail.id).subscribe(
      res => {
        this.refreshCleaningsList();
        this.toastr.success("Úklid úspěšně zrušen.", "Zrušení úklidu");
      },
      err => {
        console.log(err)
        this.toastr.error("Úklid se nepovedlo zrušit.", "Zrušení úklidu");
      }
    );
  }

  public getCleaningData(cleaningId: number) {
    this.getCleaningRequest(cleaningId).subscribe(
      res => {
        this.cleaningDetail = res as Cleaning;
      },
      err => {
        if (err.status && err.status === 404) {
          this.toastr.error("Hledaný úklid neexistuje.", "Úklid");
        } else {
          this.toastr.error("Nepodařilo se načíst informace o požadovaném úklidu.", "Úklid");
        }
      }
    );
  }

  saveBackRoute(route: string): void {
    this.cleaningsBackRoute = route;
  }

  getBackRoute(): string {
    return this.cleaningsBackRoute;
  }

  resetBackRoute(): void {
    this.cleaningsBackRoute = "..";
  }

  /**
   * Gets next events as a list of events.
   */
  getNextCleanings(): Observable<Cleaning[]> {
    return this.http.get<Cleaning[]>(`${this.CleaningsUrl}/next`);
  }

  refreshNextCleanings() {
    this.getNextCleanings().toPromise()
      .then((res: Cleaning[]) => {
        this.cleaningsList = res;
      }).catch((error: any) => {
        console.log(error);
        this.toastr.error("Nepodařilo se načíst nejbližší úklidy.", "Načtení úklidů");
        return;
      });
  }

  getFormattedFromDate(format: string = "d. M. yyyy HH:mm") {
    return formatDate(this.cleaningDetail.from, format, "cs-CZ")
  }

  getFormattedToDate(format: string = "d. M. yyyy HH:mm") {
    return formatDate(this.cleaningDetail.to, format, "cs-CZ")
  }

  openCleaningFinishConfirmationModal(cleaningDetail: Cleaning) {
    const modal = this._modalService.open(CleaningFinishConfirmationModalComponent);

    modal.result.then(
      (result) => {
        if (result == "Finished") {
          this.handleFinishCleaning(cleaningDetail);
        }
      }, (reason) => {
      }
    );
  }

  openCleaningDeletionConfirmationModal(cleaningDetail: Cleaning) {
    const modal = this._modalService.open(DeletionConfirmationModalComponent);
    modal.componentInstance.name = cleaningDetail.name;
    modal.componentInstance.type = DeletionType.Cleaning;

    modal.result.then(
      (result) => {
        if (result == "Confirm deletion") {
          this.handleRemoveCleaning(cleaningDetail);
        }
      }, (reason) => {
      }
    );
  }

  handleJoinCleaning(cleaningDetail?: Cleaning){
    if (cleaningDetail) {
      this.cleaningDetail = Object.assign({}, cleaningDetail);
    }

    this.joinCleaningRequest(this.cleaningDetail.id).subscribe(
      res => {
        this.refreshCleaningsList();
        this.toastr.success("Úklid úspěšně připojen.", "Připojení k úklidu");
      },
      err => {
        console.log(err)
        this.toastr.error("K úklidu se nepodařilo připojit.", "Připojeni k úklidu");
      }
    );
  }

  handleLeaveCleaning(cleaningDetail?: Cleaning){
    if (cleaningDetail) {
      this.cleaningDetail = Object.assign({}, cleaningDetail);
    }

    this.leaveCleaningRequest(this.cleaningDetail.id).subscribe(
      res => {
        this.refreshCleaningsList();
        this.toastr.success("Úklid úspěšně opuštěn.", "Opuštění k úklidu");
      },
      err => {
        console.log(err)
        this.toastr.error("Úklid se nepodařilo opustit.", "Opuštění úklidu");
      }
    );
  }
}