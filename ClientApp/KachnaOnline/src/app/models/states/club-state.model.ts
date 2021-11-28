// club-state.model.ts
// Author: David Chocholatý

import { MadeByUser } from "../users/made-by-user.model";

export class ClubState {
  id: number;
  state: string;
  madeByUser: MadeByUser;
  start: Date;
  plannedEnd: Date;
  note: string;
  eventId: number;
  followingState: ClubState;
  noteInternal: string;
  actualEnd: Date;
  closedByUser: MadeByUser;
}
