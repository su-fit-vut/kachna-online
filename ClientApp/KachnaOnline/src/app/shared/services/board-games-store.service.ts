import { Injectable } from '@angular/core';
import { ReservationState } from "../../models/board-games/reservation.model";

@Injectable({
  providedIn: 'root'
})
export class BoardGamesStoreService {
  // Filter and reservation storage
  players: number | undefined;
  availableOnly: boolean | undefined;
  categoryIds: number[] = [];
  currentReservation: Map<number, number> = new Map();

  // Reservation filter storage
  reservationFilter: ReservationState | undefined = undefined;

  // Manager reservation filter storage
  managerReservationFilter: ReservationState | undefined = undefined;
  onlyAssignedToMe: boolean = false;

  // Board game details page route storage
  backRoute: string = "..";

  constructor() {
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
   * Saves a back route for a detailed board game page.
   * @param route The route to save.
   */
  saveBackRoute(route: string): void {
    this.backRoute = route;
  }

  /**
   * Returns the previously saved back route for a detailed board game page.
   */
  getBackRoute(): string {
    return this.backRoute;
  }

  /**
   * Resets a previously set back route to its initial value.
   */
  resetBackRoute(): void {
    this.backRoute = "..";
  }

  /**
   * Saves the current manager filters.
   * @param filter Reservation of which states are currently shown.
   * @param assignedToMe Whether only the reservations which the current user is assigned to are shown. Nothing
   *  is saved if the value is undefined.
   */
  saveManagerFilter(filter: ReservationState | undefined, assignedToMe: boolean | undefined = undefined): void {
    this.managerReservationFilter = filter;
    if (assignedToMe !== undefined) {
      this.onlyAssignedToMe = assignedToMe;
    }
  }

  /**
   * Get the previously saved manager filters.
   */
  getManagerFilter(): [ReservationState | undefined, boolean] {
    return [this.managerReservationFilter, this.onlyAssignedToMe];
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

}
