// event.model.ts
// Author: David Chocholatý

import { ClubState } from "../states/club-state.model";

export class Event {
  id:number = -1;
  name:string = "";
  place:string = "";
  placeUrl:string = "";
  imageUrl:string = "";
  shortDescription:string = "";
  fullDescription:string = "";
  url:string = "";
  from: string = "";
  to:string = "";
  linkedPlannedStateIds: number[] = [];
  madeById:string = "";
  linkedStatesDtos: ClubState[];
}
