// board-games-service.ts
// Author: František Nečas

import { Injectable } from '@angular/core';
import { environment } from "../../../environments/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { forkJoin, Observable } from "rxjs";
import { BoardGame } from "../../models/board-games/board-game-model";
import { BoardGameCategory } from "../../models/board-games/category-model";

enum ApiPaths {
  Categories = '/categories'
}

@Injectable({
  providedIn: 'root'
})
export class BoardGamesService {

  readonly BoardGamesUrl = environment.baseApiUrl + '/boardGames';

  constructor(
    private http: HttpClient,
  ) {
  }

  getBoardGames(categories: number[], players: number | undefined,
                available: boolean | undefined): Observable<BoardGame[][]> {
    let params = new HttpParams();
    if (players != undefined) {
      params = params.set("players", players);
    }
    if (available != undefined) {
      params = params.set("available", available);
    }
    if (categories.length == 0) {
      return forkJoin([this.http.get<BoardGame[]>(this.BoardGamesUrl, {params: params})]);
    }
    let requests = [];
    for (let category of categories) {
      params = params.set("categoryId", category);
      requests.push(this.http.get<BoardGame[]>(this.BoardGamesUrl, {params: params}))
    }
    return forkJoin(requests);
  }

  getCategories(): Observable<BoardGameCategory[]> {
    let url = `${this.BoardGamesUrl}${ApiPaths.Categories}`
    return this.http.get<BoardGameCategory[]>(url);
  }
}
