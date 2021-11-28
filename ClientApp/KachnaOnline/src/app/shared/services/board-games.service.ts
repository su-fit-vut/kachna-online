// board-games-service.ts
// Author: František Nečas

import { Injectable } from '@angular/core';
import { environment } from "../../../environments/environment";
import { HttpClient, HttpParams, HttpResponse } from "@angular/common/http";
import { forkJoin, Observable } from "rxjs";
import { BoardGame } from "../../models/board-games/board-game-model";
import { BoardGameCategory } from "../../models/board-games/category-model";
import { Reservation, ReservationState } from "../../models/board-games/reservation-model";
import { ReservationItemState } from "../../models/board-games/reservation-item-model";
import { ReservationEventType } from "../../models/board-games/reservation-item-event-model";

enum ApiPaths {
  Categories = '/categories',
  Reservations = '/reservations'
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
  currentReservation: Map<number, number> = new Map();

  // Reservation filter storage
  reservationFilter: ReservationState | undefined = undefined;

  readonly BoardGamesUrl = environment.baseApiUrl + '/boardGames';
  readonly CategoriesUrl = `${this.BoardGamesUrl}${ApiPaths.Categories}`;
  readonly ReservationsUrl = `${this.BoardGamesUrl}${ApiPaths.Reservations}`;

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
                         currentReservation: Map<number, number>): void {
    this.players = players;
    this.availableOnly = availableOnly;
    this.categoryIds = categoryIds;
    this.currentReservation = currentReservation;
  }

  /**
   * Resets the currently saved reservation.
   */
  resetSavedReservation(): void {
    this.currentReservation = new Map();
  }

  /**
   * Return the saved state of board game page.
   */
  getBoardGamePageState(): [number | undefined, boolean | undefined, number[], Map<number, number>] {
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
      requests.push(this.http.get<BoardGame[]>(this.BoardGamesUrl, {params: params}));
    }
    return forkJoin(requests);
  }

  /**
   * Returns an observable of a board game with the given ID.
   * @param id ID of the board game to get.
   */
  getBoardGame(id: number): Observable<BoardGame> {
    return this.http.get<BoardGame>(`${this.BoardGamesUrl}/${id}`);
  }

  /**
   * Returns an observable array of categories.
   */
  getCategories(): Observable<BoardGameCategory[]> {
    let url = this.CategoriesUrl;
    return this.http.get<BoardGameCategory[]>(url);
  }

  // Reservations

  /**
   * Saves a reservation filter.
   * @param filter Filter to save.
   */
  saveReservationFilter(filter: ReservationState | undefined): void {
    this.reservationFilter = filter;
  }

  /**
   * Returns the previously saved reservation filter.
   */
  getReservationFilter(): ReservationState | undefined {
    return this.reservationFilter;
  }

  /**
   * Creates a reservation.
   * @param toReserve Map of game IDs to the count to reserve.
   * @param note Note specified by the user.
   * @returns Observable An observable of the created reservation.
   */
  reserve(toReserve: Map<number, number>, note: string): Observable<Reservation> {
    let ids: number[] = [];
    for (let [game, count] of toReserve) {
      ids = ids.concat(Array(count).fill(game));
    }
    let newReservation = {noteUser: note, boardGameIds: ids};
    return this.http.post<Reservation>(this.ReservationsUrl, newReservation);
  }

  /**
   * Returns a list of reservations of the currently signed-in user.
   * @param state Overall state of the reservation to filter by. Undefined for all reservations.
   */
  getReservations(state: ReservationState | undefined): Observable<Reservation[]> {
    let params = new HttpParams();
    if (state != undefined) {
      params = params.set("state", state);
    }
    return this.http.get<Reservation[]>(this.ReservationsUrl, {params: params});
  }

  /**
   * Returns a reservation with the given ID.
   * @param id ID of the reservation to return.
   */
  getReservation(id: number): Observable<Reservation> {
    return this.http.get<Reservation>(`${this.ReservationsUrl}/${id}`);
  }

  /**
   * Updates a user note in a reservation.
   * @param reservationId ID of the reservation to update.
   * @param newNote New note.
   */
  setReservationUserNote(reservationId: number, newNote: string): Observable<any> {
    return this.http.put<any>(`${this.ReservationsUrl}/${reservationId}/note`, {noteUser: newNote});
  }

  /**
   * Updates a state of a reservation item.
   * @param reservationId ID of the reservation the item is in.
   * @param itemId ID of the item to update state of.
   * @param type The type of event to perform.
   */
  updateReservationState(reservationId: number, itemId: number, type: ReservationEventType): Observable<any> {
    let params = new HttpParams().set("type", type);
    return this.http.post<any>(`${this.ReservationsUrl}/${reservationId}/${itemId}/events`, {}, {params: params});
  }
}
