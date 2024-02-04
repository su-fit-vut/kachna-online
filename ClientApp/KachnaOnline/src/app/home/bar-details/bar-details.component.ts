import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ClubState } from "../../models/states/club-state.model";
import { ClubInfoService } from "../../shared/services/club-info.service";
import { PrestigeLeaderboardItem } from "../../models/clubinfo/leaderboard.model";
import { Subscription, timer } from "rxjs";

@Component({
  selector: 'app-bar-details',
  templateUrl: './bar-details.component.html',
  styleUrls: ['./bar-details.component.css', '../home.component.css']
})
export class BarDetailsComponent implements OnInit, OnDestroy {

  constructor(public service: ClubInfoService) { }

  @Input() state: ClubState;

  leaderboardCollapsed: boolean = true;

  todayPrestige: PrestigeLeaderboardItem[];
  semesterPrestige: PrestigeLeaderboardItem[];

  reloadSubscription: Subscription;

  ngOnInit(): void {

  }

  ngOnDestroy(): void {
    if (this.reloadSubscription != undefined && !this.reloadSubscription.closed) {
      this.reloadSubscription?.unsubscribe();
    }
  }

  startLoadingPrestige(): void {
    if (this.leaderboardCollapsed) {
      this.reloadSubscription?.unsubscribe();
    } else {
      this.reloadSubscription = timer(0, 10000).subscribe(_ => {
        if (!this.leaderboardCollapsed) {
          this.service.getTodayLeaderboard().subscribe(result => this.todayPrestige = result);
          this.service.getSemesterLeaderboard().subscribe(result => this.semesterPrestige = result);
        }
      });
    }
  }
}
