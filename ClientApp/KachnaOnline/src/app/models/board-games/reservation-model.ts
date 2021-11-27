// reservation-model.ts
// Author: František Nečas

import { MadeByUser } from "../users/made-by-user-model";
import { ReservationItem } from "./reservation-item-model";

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
