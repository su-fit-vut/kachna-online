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

/**
 * A service for interacting with board games API.
 */
@Injectable({
  providedIn: 'root'
})
export class BoardGamesService {
  // Filter and reservation storage
  players: number | undefined;
  availableOnly: boolean | undefined;
  categoryIds: number[] = [];
  currentReservation: { [id: number]: number } = {};

  readonly BoardGamesUrl = environment.baseApiUrl + '/boardGames';

  constructor(
    private http: HttpClient,
  ) {
  }

  /**
   * Saves the current board game page states (filters) so that the component can be destroyed and restored later.
   * @param players Number of players selected.
   * @param availableOnly Whether availability filtering is applied.
   * @param categoryIds IDs of categories shown.
   * @param currentReservation Current state of ongoing reservation.
   */
  saveBoardGamePageState(players: number | undefined, availableOnly: boolean | undefined, categoryIds: number[],
                         currentReservation: { [id: number]: number }): void {
    this.players = players;
    this.availableOnly = availableOnly;
    this.categoryIds = categoryIds;
    this.currentReservation = currentReservation;
  }

  /**
   * Return the saved state of board game page.
   */
  getBoardGamePageState(): [number | undefined, boolean | undefined, number[], { [id: number]: number}] {
    return [this.players, this.availableOnly, this.categoryIds, this.currentReservation];
  }

  /**
   * Returns an observable array of arrays of board games.
   *
   * The top-level array contains arrays corresponding to each requested category.
   * @param categories Categories to get.
   * @param players Limit the search based on the number of players.
   * @param available Limit the search based on availability.
   */
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

  /**
   * Returns an observable array of categories.
   */
  getCategories(): Observable<BoardGameCategory[]> {
    let url = `${this.BoardGamesUrl}${ApiPaths.Categories}`
    return this.http.get<BoardGameCategory[]>(url);
  }
}