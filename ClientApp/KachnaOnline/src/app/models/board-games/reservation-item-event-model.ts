// reservation-item-event-model.ts
// Author: František Nečas

/**
 * Represents a type of action with a reservation item.
 */
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
