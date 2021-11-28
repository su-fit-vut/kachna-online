import { Pipe, PipeTransform } from '@angular/core';
import { ClubStateTypes } from "../../models/states/club-state-types.model";

@Pipe({
  name: 'stateLoc'
})
export class StateLocPipe implements PipeTransform {

  names: { [id in ClubStateTypes]: string; } = {
    OpenChillzone: "chillzóna",
    OpenBar: "otevřeno s barem",
    Private: "soukromá akce",
    Closed: "zavřeno"
  };

  transform(value: ClubStateTypes): string {
    return this.names[value];
  }

}
