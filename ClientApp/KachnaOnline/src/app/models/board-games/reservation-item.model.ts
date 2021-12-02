// reservation-item.model.ts
// Author: František Nečas

import { MadeByUser } from "../users/made-by-user.model";
import { ReservationEventType } from "./reservation-item-event.model";

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
 * Contains basic information about a reserved game.
 */
export class ReservedGame {
  id: number
  name: string
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
  lastEventType: ReservationEventType
}
