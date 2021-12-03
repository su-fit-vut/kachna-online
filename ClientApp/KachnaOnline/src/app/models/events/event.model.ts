// event.model.ts
// Author: David Chocholatý

import { BaseEvent } from "./base-event.model";

export class Event extends BaseEvent {
  from: Date = new Date();
  to: Date = new Date();
}
