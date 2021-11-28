// reservation-item-model.ts
// Author: František Nečas

import { ReservedGame } from "./reserved-game-model";
import { MadeByUser } from "../users/made-by-user-model";

/**
 * Overall state of a reservation item.
 */
export enum ReservationItemState {
  New = "New",
  Cancelled = "Cancelled",
  Assigned = "Assigned",
  HandedOver = "HandedOver",
  Done = "Done",
  Expired = "Expired"
}

/**
 * Represents a single item in a reservation.
 */
export class ReservationItem {
  id: number
  boardGame: ReservedGame
  expiresOn: Date
  assignedTo: MadeByUser
  state: ReservationItemState
}
