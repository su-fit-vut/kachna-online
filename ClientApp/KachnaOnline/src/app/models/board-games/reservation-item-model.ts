// reservation-item-model.ts
// Author: František Nečas

import { ReservedGame } from "./reserved-game-model";
import { MadeByUser } from "../users/made-by-user-model";

export enum ReservationItemState {
  New = "New",
  Cancelled = "Cancelled",
  Assigned = "Assigned",
  HandedOver = "HandedOver",
  Done = "Done",
  Expired = "Expired"
}

export class ReservationItem {
  id: number
  boardGame: ReservedGame
  expiresOn: Date
  assignedTo: MadeByUser
  state: ReservationItemState
}
