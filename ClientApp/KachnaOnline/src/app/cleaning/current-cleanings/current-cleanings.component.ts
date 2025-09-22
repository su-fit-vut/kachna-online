import { CleaningsService } from '../../shared/services/cleanings.service';
import { Component, OnInit } from '@angular/core';
import { Cleaning } from "../../models/cleaning/cleaning.model";
import { Router } from "@angular/router";
import { AuthenticationService } from "../../shared/services/authentication.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { CleaningItem } from "../cleaning-item/cleaning-item.component";
import { StatesService } from "../../shared/services/states.service";
import { forkJoin, Observable } from "rxjs";
import { ClubStateTypes } from "../../models/states/club-state-types.model";
import { endWith } from "rxjs/operators";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'app-current-cleanings',
  templateUrl: './current-cleanings.component.html',
  styleUrls: ['./current-cleanings.component.css']
})
export class CurrentCleaningsComponent implements OnInit {
  cleanings: Cleaning[] = [];
  grouped: {[place:string]: Cleaning[]} = {};
  currentMonth: Date = new Date();
  loading: boolean = false;
  now: Date = new Date();

  constructor(
    public cleaningsService: CleaningsService,
    private stateService: StatesService,
    private toastrService: ToastrService,
    private router: Router,
    public authenticationService: AuthenticationService,
    private _modalService: NgbModal,
  ) { }


  ngOnInit(): void {
    this.loading = true;

    this.cleaningsService.getUnfinishedCleaningsRequest().subscribe(
      (res: Cleaning[]) => {
        this.makeCleanings(res);
      },
      (_) => {
        this.toastrService.error("Nepodařilo se stáhnout úklidy.", "Stažení úklidů")
      }
    );
  }

  openCleaningDetail(cleaningDetail: Cleaning) {
    this.router.navigate([`/cleanings/${cleaningDetail.id}`]).then();
  }

  onJoinButtonClicked(selectedCleaningDetail: Cleaning) {
    this.cleaningsService.handleJoinCleaning(selectedCleaningDetail);
    window.location.reload();
  }
    
  onLeaveButtonClicked(selectedCleaningDetail: Cleaning) {
    this.cleaningsService.handleLeaveCleaning(selectedCleaningDetail);
    window.location.reload();
  }

  onModifyButtonClicked(selectedCleaningDetail: Cleaning) {
    this.router.navigate([`/cleanings/${selectedCleaningDetail.id}/edit`]).then();
  }
  
  onFinishButtonClicked(selectedCleaningDetail: Cleaning) {
    this.cleaningsService.openCleaningFinishConfirmationModal(selectedCleaningDetail);
  }

  makeCleanings(cleaningModels: Cleaning[]): void {
    this.cleanings = [];
    this.grouped = {};
    let waiting: Observable<any>[] = [];

    for (let c of cleaningModels) {

      if(c.finished == false) {
        this.cleanings.push(c);
        if (!this.grouped[c.place]) {
          this.grouped[c.place] = [];
        }
        this.grouped[c.place].push(c);
      }
    }

    forkJoin(waiting).pipe(endWith(null)).subscribe(_ => {
      // this.updateNextCleanings();
      this.loading = false;
    });
  }

  sortCleanings(cleanings: CleaningItem[]): CleaningItem[] {
    return cleanings.sort((a, b) => a.from.getTime() - b.from.getTime());
  }

  update(source: { to: Date }[], target: { to: Date }[]): void {
    const now = new Date().getTime();
    target.length = 0;

    for (let e of source) {
      if (e.to.getTime() < now)
        continue;

      target.push(e);
    }
  }
}
