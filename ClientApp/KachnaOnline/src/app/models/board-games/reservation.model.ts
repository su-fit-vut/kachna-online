// reservation.model.ts
// Author: František Nečas

import { MadeByUser } from "../users/made-by-user-model";
import { ReservationItem } from "./reservation-item.model";

/**
 * Overall state of the reservation which can be filtered by.
 */
export enum ReservationState {
  New = "New",
  Current = "Current",
  Done = "Done",
  Expired = "Expired"
}

/**
 * Model of a reservation.
 */
export class Reservation {
  id: number
  madeBy: MadeByUser
  madeOn: Date
  noteUser: string
  items: ReservationItem[]
}
