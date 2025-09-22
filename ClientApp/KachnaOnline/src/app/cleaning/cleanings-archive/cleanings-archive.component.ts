import { Component, OnInit } from '@angular/core';
import { Cleaning } from "../../models/cleaning/cleaning.model";
import { CleaningsService } from "../../shared/services/cleanings.service";
import { ToastrService } from "ngx-toastr";
import { Router } from "@angular/router";
import { AuthenticationService } from "../../shared/services/authentication.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { throwError } from "rxjs";

@Component({
  selector: 'app-cleanings-archive',
  templateUrl: './cleanings-archive.component.html',
  styleUrls: ['./cleanings-archive.component.css']
})
export class CleaningsArchiveComponent implements OnInit {
  cleanings: Cleaning[] = [];
  maxCleaningInstructionsChars: number = 50;
  now: Date = new Date();

  constructor(
    public cleaningsService: CleaningsService,
    private toastrService: ToastrService,
    private router: Router,
    public authenticationService: AuthenticationService,
    private _modalService: NgbModal,
  ) { }

  ngOnInit(): void {
    this.cleaningsService.getMonthCleanings(this.now).subscribe(
      res => this.setCleanings(res),
      err => {
        this.toastrService.error("Stažení úklidů selhalo.", "Stažení úklidů");
        return throwError(err);
      });
  }

  openCleaningDetail(cleaningDetail: Cleaning) {
    this.router.navigate([`/cleanings/${cleaningDetail.id}`]).then();
  }

  onDeleteButtonClicked(selectedCleaningDetail: Cleaning) {
    this.cleaningsService.openCleaningDeletionConfirmationModal(selectedCleaningDetail);
  }

  onModifyButtonClicked(selectedCleaningDetail: Cleaning) {
    this.router.navigate([`/cleanings/${selectedCleaningDetail.id}/edit`]).then();
  }

  yearChanged(year: Date) {
    this.cleaningsService.getYearCleanings(year).subscribe(
      res => this.setCleanings(res),
      err => {
        this.toastrService.error("Stažení úklidů selhalo.", "Stažení úklidů");
        return throwError(err);
      });
  }

  setCleanings(cleaningModels: Cleaning[]): void {
    this.cleanings = this.sortCleanings(cleaningModels);
  }

  sortCleanings(cleanings: Cleaning[]): Cleaning[] {
    return cleanings.sort((a, b) => a.from.getTime() - b.from.getTime());
  }
}
