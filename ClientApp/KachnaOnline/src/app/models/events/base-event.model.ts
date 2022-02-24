import { ClubState } from "../states/club-state.model";

export class BaseEvent {
  id: number = -1;
  name: string = "";
  place: string = "";
  placeUrl: string = "";
  imageUrl: string = "";
  shortDescription: string = "";
  fullDescription: string = "";
  url: string = "";
  linkedPlannedStateIds: number[] | null = [];
  madeById: string = "";
  linkedStatesDtos: ClubState[] = [];
}
