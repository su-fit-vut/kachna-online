// board-games-service.ts
// Author: František Nečas

import { Injectable } from '@angular/core';
import { environment } from "../../../environments/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { BoardGame } from "../../models/board-games/board-game-model";

@Injectable({
  providedIn: 'root'
})
export class BoardGamesService {

  readonly BoardGamesUrl = environment.baseApiUrl + '/boardGames';

  constructor(
    private http: HttpClient,
  ) {
  }

  getBoardGames(categoryId: number | undefined, players: number | undefined,
                available: boolean | undefined): Observable<BoardGame[]> {
    let params = new HttpParams();
    if (categoryId != undefined) {
      params.set("categoryId", categoryId);
    }
    if (players != undefined) {
      params.set("players", players);
    }
    if (available != undefined) {
      params.set("available", available);
    }
    return this.http.get<BoardGame[]>(this.BoardGamesUrl, {params: params});
  }
}
