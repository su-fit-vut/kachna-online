import { Component, Input, OnInit } from '@angular/core';
import { PrestigeLeaderboardItem } from "../../../models/clubinfo/leaderboard.model";

@Component({
  selector: 'app-prestige-table',
  templateUrl: './prestige-table.component.html',
  styleUrls: ['./prestige-table.component.css']
})
export class PrestigeTableComponent implements OnInit {

  @Input() title: string;
  @Input() leaderboard: PrestigeLeaderboardItem[];

  constructor() { }

  ngOnInit(): void {
  }

}
