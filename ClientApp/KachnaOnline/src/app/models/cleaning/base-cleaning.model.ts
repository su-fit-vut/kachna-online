export class BaseCleaning {
  id: number = -1;
  name: string = "";
  place: string = "";
  cleaningInstructions: string = "";
  madeById: string = "";
  assignedUsersIds: number[] | null = [];
  idsToUsername: {item1: number, item2: string}[] = [];
  idealParticipantsCount: number | null = null;
  finished: boolean = false;
}
