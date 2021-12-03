// board-games-service.ts
// Author: František Nečas

import { Injectable } from '@angular/core';
import { environment } from "../../../environments/environment";
import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from "@angular/common/http";
import { forkJoin, Observable } from "rxjs";
import { BoardGame } from "../../models/board-games/board-game.model";
import { BoardGameCategory } from "../../models/board-games/board-game-category.model";
import { Reservation, ReservationState } from "../../models/board-games/reservation.model";
import { ReservationEventType, ReservationItemEvent } from "../../models/board-games/reservation-item-event.model";
import { ReservationItem } from "../../models/board-games/reservation-item.model";

enum ApiPaths {
  Categories = '/categories',
  Reservations = '/reservations'
}

enum ReservationApiPaths {
  Note = '/note',
  NoteInternal = '/noteInternal',
  Events = '/events',
  All = '/all',
  AssignedToMe = '/assignedTo/me'
}

/**
 * A service for interacting with board games API.
 */
@Injectable({
  providedIn: 'root'
})
export class BoardGamesService {
  readonly BoardGamesUrl = environment.baseApiUrl + '/boardGames';
  readonly CategoriesUrl = `${this.BoardGamesUrl}${ApiPaths.Categories}`;
  readonly ReservationsUrl = `${this.BoardGamesUrl}${ApiPaths.Reservations}`;

  constructor(
    private http: HttpClient,
  ) {
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
   * Creates a new board game and returns its observable.
   * @param game Game to create.
   */
  createBoardGame(game: object): Observable<BoardGame> {
    return this.http.post<BoardGame>(`${this.BoardGamesUrl}`, game);
  }

  /**
   * Updates a board game with the given ID.
   * @param id ID of the game to update
   * @param game New game data.
   */
  updateBoardGame(id: number, game: object): Observable<any> {
    return this.http.put<any>(`${this.BoardGamesUrl}/${id}`, game);
  }

  /**
   *
   * @param id
   * @param inStock
   * @param unavailable
   * @param visible
   */
  updateBoardGameStock(id: number, inStock: number, unavailable: number, visible: boolean): Observable<any> {
    return this.http.put<any>(`${this.BoardGamesUrl}/${id}/stock`, {inStock: inStock,
      unavailable: unavailable, visible: visible});
  }

  // Categories

  /**
   * Returns an observable array of categories.
   */
  getCategories(): Observable<BoardGameCategory[]> {
    let url = this.CategoriesUrl;
    return this.http.get<BoardGameCategory[]>(url);
  }

  /**
   * Returns a category with the given ID.
   * @param id ID of the category to return.
   */
  getCategory(id: number): Observable<BoardGameCategory> {
    return this.http.get<BoardGameCategory>(`${this.CategoriesUrl}/${id}`);
  }

  /**
   * Creates a new category and returns it as an observable.
   * @param name Name of the category
   * @param hex RGB color of the category
   */
  createCategory(name: string, hex: string): Observable<BoardGameCategory> {
    if (hex[0] == '#') {
      hex = hex.slice(1);
    }
    return this.http.post<BoardGameCategory>(this.CategoriesUrl, {name: name, colourHex: hex});
  }

  /**
   * Updates an existing category.
   * @param id ID of the category to update.
   * @param name New name of the category
   * @param hex New RGB color of the category
   */
  updateCategory(id: number, name: string, hex: string): Observable<BoardGameCategory> {
    if (hex[0] == '#') {
      hex = hex.slice(1);
    }
    return this.http.put<BoardGameCategory>(`${this.CategoriesUrl}/${id}`, {name: name, colourHex: hex})
  }

  /**
   * Deletes a category
   * @param id ID of the category to delete.
   */
  deleteCategory(id: number): Observable<any> {
    return this.http.delete<any>(`${this.CategoriesUrl}/${id}`);
  }

  // Reservations

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
   * Creates a reservation for a user.
   * @param toReserve Map of game IDs to the count to reserve.
   * @param noteInternal Internal note to save along the reservation.
   * @param userId ID of the user to reserve for.
   */
  reserveForUser(toReserve: Map<number, number>, noteInternal: string, userId: number): Observable<Reservation> {
    let ids: number[] = [];
    for (let [game, count] of toReserve) {
      ids = ids.concat(Array(count).fill(game));
    }
    let newReservation = {noteInternal: noteInternal, boardGameIds: ids};
    return this.http.post<Reservation>(`${this.ReservationsUrl}/madeFor/${userId}`, newReservation);
  }

  addToReservation(reservationId: number, toReserve: Map<number, number>): Observable<any> {
    let ids: number[] = [];
    for (let [game, count] of toReserve) {
      ids = ids.concat(Array(count).fill(game));
    }
    return this.http.post(`${this.ReservationsUrl}/${reservationId}/items`, ids);
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
   * Returns a list of all reservations.
   * @param state Overall state of the reservation to filter by. Undefined for all reservations.
   */
  getAllReservations(state: ReservationState | undefined): Observable<Reservation[]> {
    let params = new HttpParams();
    if (state != undefined) {
      params = params.set("state", state);
    }
    return this.http.get<Reservation[]>(`${this.ReservationsUrl}${ReservationApiPaths.All}`, {params: params});
  }

  /**
   * Returns a list of all reservations assigned to the currently signed in user.
   * @param state Overall state of the reservation to filter by. Undefined for all reservations.
   */
  getAllReservationsAssignedToMe(state: ReservationState | undefined): Observable<Reservation[]> {
    let params = new HttpParams();
    if (state != undefined) {
      params = params.set("state", state);
    }
    return this.http.get<Reservation[]>(
      `${this.ReservationsUrl}${ReservationApiPaths.All}${ReservationApiPaths.AssignedToMe}`, {params: params});
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
    return this.http.put<any>(`${this.ReservationsUrl}/${reservationId}${ReservationApiPaths.Note}`,
      JSON.stringify(newNote), {headers: new HttpHeaders({'Content-Type': 'application/json'})});
  }

  /**
   * Updates an internal note in a reservation.
   * @param reservationId ID of the reservation to update.
   * @param newNote New note.
   */
  setReservationInternalNote(reservationId: number, newNote: string): Observable<any> {
    return this.http.put<any>(`${this.ReservationsUrl}/${reservationId}${ReservationApiPaths.NoteInternal}`,
      JSON.stringify(newNote), {headers: new HttpHeaders({'Content-Type': 'application/json'})});
  }

  /**
   * Updates a state of a reservation item.
   * @param reservationId ID of the reservation the item is in.
   * @param itemId ID of the item to update state of.
   * @param type The type of event to perform.
   */
  updateReservationState(reservationId: number, itemId: number, type: ReservationEventType): Observable<any> {
    let params = new HttpParams().set("type", type);
    return this.http.post<any>(`${this.ReservationsUrl}/${reservationId}/${itemId}${ReservationApiPaths.Events}`,
      {}, {params: params});
  }

  /**
   * Returns an observable of a reservation item.
   * @param reservationId ID of the reservation the item is in.
   * @param itemId ID of the item to get.
   */
  getReservationItem(reservationId: number, itemId: number): Observable<ReservationItem> {
    return this.http.get<ReservationItem>(
      `${this.ReservationsUrl}/${reservationId}/${itemId}`);
  }

  /**
   * Returns an observable of an item history of a reservation.
   * @param reservationId ID of the reservation the item is in.
   * @param itemId ID of the item to get the history of.
   */
  getReservationItemHistory(reservationId: number, itemId: number): Observable<ReservationItemEvent[]> {
    return this.http.get<ReservationItemEvent[]>(
      `${this.ReservationsUrl}/${reservationId}/${itemId}${ReservationApiPaths.Events}`);
  }
}
