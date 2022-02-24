import { ClubStateTypes } from "./club-state-types.model";

export class StateModification {
  madeById?: number | null;
  state?: ClubStateTypes | null;
  start?: string | null;
  plannedEnd: string | null;
  noteInternal: string | null;
  notePublic: string | null;
}
