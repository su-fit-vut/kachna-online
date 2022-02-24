import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { PrestigeLeaderboardItem } from "../../models/clubinfo/leaderboard.model";

@Injectable({
  providedIn: 'root'
})
export class ClubInfoService {

  readonly ClubInfoUrl = environment.baseApiUrl + '/club';

  constructor(
    private http: HttpClient
  ) {}

  getOffer(): Observable<any> {
    return this.http.get<any>(`${this.ClubInfoUrl}/offer`);
  }

  getTodayLeaderboard(): Observable<PrestigeLeaderboardItem[]> {
    return this.http.get<PrestigeLeaderboardItem[]>(`${this.ClubInfoUrl}/leaderboard/today`);
  }

  getSemesterLeaderboard(): Observable<PrestigeLeaderboardItem[]> {
    return this.http.get<PrestigeLeaderboardItem[]>(`${this.ClubInfoUrl}/leaderboard/semester`);
  }

}
