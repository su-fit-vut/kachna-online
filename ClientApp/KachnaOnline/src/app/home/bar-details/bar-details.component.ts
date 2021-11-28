import { Component, Input, OnInit } from '@angular/core';
import { ClubState } from "../../models/states/club-state.model";
import { ClubInfoService } from "../../shared/services/club-info.service";
import { PrestigeLeaderboardItem } from "../../models/clubinfo/leaderboard.model";

@Component({
  selector: 'app-bar-details',
  templateUrl: './bar-details.component.html',
  styleUrls: ['./bar-details.component.css', '../home.component.css']
})
export class BarDetailsComponent implements OnInit {

  constructor(public service: ClubInfoService) { }

  @Input() state: ClubState;

  currentOfferCollapsed: boolean = true;
  leaderboardCollapsed: boolean = true;

  todayPrestige: PrestigeLeaderboardItem[];
  semesterPrestige: PrestigeLeaderboardItem[];

  ngOnInit(): void {
  }

  loadPrestige(): void {
    if (!this.todayPrestige) {
      this.service.getTodayLeaderboard().subscribe(result => this.todayPrestige = result);
    }

    if (!this.semesterPrestige) {
      this.service.getSemesterLeaderboard().subscribe(result => this.semesterPrestige = result);
    }
  }

}
