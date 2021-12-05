// day-loc.pipe.ts
// Author: Ondřej Ondryáš

import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dayLoc'
})
export class DayLocPipe implements PipeTransform {

  namesStr: { [id: string]: string; } = {
    "Monday": "pondělí",
    "Tuesday": "úterý",
    "Wednesday": "středa",
    "Thursday": "čtvrtek",
    "Friday": "pátek",
    "Saturday": "sobota",
    "Sunday": "neděle"
  };

  namesInt: { [id: number]: string; } = {
    1: "pondělí",
    2: "úterý",
    3: "středa",
    4: "čtvrtek",
    5: "pátek",
    6: "sobota",
    0: "neděle"
  };

  transform(value: string | number): string {
    if (typeof value == 'string') {
      return this.namesStr[value];
    } else {
      return this.namesInt[value];
    }
  }

}
