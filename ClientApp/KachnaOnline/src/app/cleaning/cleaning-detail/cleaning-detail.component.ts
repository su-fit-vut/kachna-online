import { ToastrService } from 'ngx-toastr';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { AuthenticationService } from "../../shared/services/authentication.service";
import { environment } from "../../../environments/environment";
import { CleaningsService } from 'src/app/shared/services/cleanings.service';
import { Cleaning } from 'src/app/models/cleaning/cleaning.model';

@Component({
  selector: 'app-cleaning-detail',
  templateUrl: './cleaning-detail.component.html',
  styleUrls: ['./cleaning-detail.component.css']
})
export class CleaningDetailComponent implements OnInit {
  cleaning: Cleaning = new Cleaning();
  activateEditCleaningModal: boolean = false;
  now: Date = new Date();


  constructor(
    public cleaningsService: CleaningsService,
    private toastrService: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
    public authenticationService: AuthenticationService,
    ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      let cleaningId = Number(params.get('cleaningId'));
      this.cleaningsService.getCleaningData(cleaningId);
    });
  }

  onModifyButtonClicked() {
    this.route.paramMap.subscribe(params => {
      let cleaningId = Number(params.get('cleaningId'));
      this.router.navigate([`/cleanings/${cleaningId}/edit`]).then();
    });
  }

  onCloseModalClicked() {
    this.activateEditCleaningModal = false;
  }

  onDeleteButtonClicked() {
    this.cleaningsService.openCleaningDeletionConfirmationModal(this.cleaningsService.cleaningDetail);
  }

  onJoinButtonClicked() {
    this.cleaningsService.handleJoinCleaning(this.cleaningsService.cleaningDetail);
    window.location.reload();
  }
    
  onLeaveButtonClicked() {
    this.cleaningsService.handleLeaveCleaning(this.cleaningsService.cleaningDetail);
    window.location.reload();
  }

  onManageLinkedStatesClicked() {
    this.route.paramMap.subscribe(params => {
      let cleaningId = Number(params.get('cleaningId'));
      this.router.navigate([`/cleaning/${cleaningId}/linked-states`]).then();
    });
  }
}
