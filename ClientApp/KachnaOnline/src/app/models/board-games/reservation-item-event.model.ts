// reservation-item-event.model.ts
// Author: František Nečas

/**
 * Represents a type of action with a reservation item.
 */
import { MadeByUser } from "../users/made-by-user-model";

export enum ReservationEventType {
  Created = "Created",
  Cancelled = "Cancelled",
  Assigned = "Assigned",
  HandedOver = "HandedOver",
  ExtensionRequested = "ExtensionRequested",
  ExtensionGranted = "ExtensionGranted",
  ExtensionRefused = "ExtensionRefused",
  Returned = "Returned"
}

/**
 * Represents a state in change of a reservation item.
 */
export class ReservationItemEvent {
  madeBy: MadeByUser
  madeOn: Date
  type: ReservationEventType
  // May be null if the expiration did not change.
  newExpiryDate: Date
}
