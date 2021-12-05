// month-loc.pipe.ts
// Author: Ondřej Ondryáš

import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'monthLoc'
})
export class MonthLocPipe implements PipeTransform {

  namesInt: { [id: number]: string; } = {
    0: "leden",
    1: "únor",
    2: "březen",
    3: "duben",
    4: "květen",
    5: "červen",
    6: "červenec",
    7: "srpen",
    8: "září",
    9: "říjen",
    10: "listopad",
    11: "prosinec"
  };

  transform(value: number): string {
    return this.namesInt[value];
  }

}
