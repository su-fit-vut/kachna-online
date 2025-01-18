import { Pipe, PipeTransform } from '@angular/core';
import { ClubStateTypes } from "../../models/states/club-state-types.model";

@Pipe({
  name: 'stateLoc'
})
export class StateLocPipe implements PipeTransform {

  names: { [id in ClubStateTypes]: string; } = {
    OpenEvent: "veřejná akce",
    OpenBar: "otevřeno s barem",
    Private: "zavřeno – soukromá akce",
    Closed: "zavřeno",
    OpenTearoom: "čajovna",
    OpenAll: "otevřeno pro všechny (bez baru)"
  };

  transform(value: ClubStateTypes): string {
    return this.names[value];
  }

}
