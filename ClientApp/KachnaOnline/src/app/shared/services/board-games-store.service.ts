import { Injectable } from '@angular/core';
import { ReservationState } from "../../models/board-games/reservation.model";

/**
 * Defines behaviour of the board games overview page.
 */
export enum BoardGamePageState {
  Normal, // Regular user creating a reservation.
  ManagerReservation, // Manager is creating a reservation for a user.
  AddingGames // Manager is adding games to an existing reservation.
}

@Injectable({
  providedIn: 'root'
})
export class BoardGamesStoreService {
  // Filter and reservation storage
  players: number | undefined;
  availableOnly: boolean | undefined;
  categoryIds: number[] = [];
  currentReservation: Map<number, number> = new Map();
  pageMode: BoardGamePageState = BoardGamePageState.Normal;

  // Reservation filter storage
  reservationFilter: ReservationState | undefined = undefined;

  // Manager reservation filter storage
  managerReservationFilter: ReservationState | undefined = undefined;
  onlyAssignedToMe: boolean = false;

  // Board game details page route storage
  backRoute: string = "..";

  // ID of the reservation we are adding items to
  reservationId: number;

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
   * Sets a mode to use in the future on the board games page.
   * @param mode The mode to use.
   */
  setPageMode(mode: BoardGamePageState): void {
    this.pageMode = mode;
  }

  /**
   * Returns the previously saved page mode (or the default normal mode).
   */
  getPageMode(): BoardGamePageState {
    return this.pageMode;
  }

  /**
   * Save reservation ID to return to once items are added.
   * @param id ID of the reservation to add items to and then return to.
   */
  saveReservationId(id: number): void {
    this.reservationId = id;
  }

  /**
   * Returns the previously save reservation ID.
   */
  getReservationId(): number {
    return this.reservationId;
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
