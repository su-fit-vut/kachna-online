// event.model.ts
// Author: David Chocholatý

import { ClubState } from "../states/club-state.model";

export class Event {
  id: number = -1;
  name: string = "";
  place: string = "";
  placeUrl: string = "";
  imageUrl: string = "";
  shortDescription: string = "";
  fullDescription: string = "";
  url: string = "";
  from: Date = new Date();
  to: Date = new Date();
  linkedPlannedStateIds: number[] | null = [];
  madeById: string = "";
  linkedStatesDtos: ClubState[];
}
