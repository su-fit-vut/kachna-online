// club-state.model.ts
// Author: Ondřej Ondryáš

import { MadeByUser } from "../users/made-by-user.model";
import { ClubStateTypes } from "./club-state-types.model";

export class ClubState {
  id: number;
  state: ClubStateTypes;
  madeByUser: MadeByUser;
  start: Date;
  plannedEnd: Date;
  note: string | null;
  noteInternal: string | null;
  eventId: number | null;
  followingState: ClubState | null;
  actualEnd: Date | null;
  closedByUser: MadeByUser | null;
}
