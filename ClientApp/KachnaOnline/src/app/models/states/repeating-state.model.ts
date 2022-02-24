import { MadeByUser } from "../users/made-by-user.model";
import { ClubStateTypes } from "./club-state-types.model";
import { Time } from "@angular/common";

export class RepeatingState {
  state: ClubStateTypes;
  dayOfWeek: string;
  effectiveFrom: Date;
  effectiveTo: Date;
  timeFrom: string;
  timeTo: string;
  note: string | null;
  id: number;
  madeByUser: MadeByUser;
  noteInternal: string | null;
}
