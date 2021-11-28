import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dayLoc'
})
export class DayLocPipe implements PipeTransform {

  names: { [id: string]: string; } = {
    "Monday": "pondělí",
    "Tuesday": "úterý",
    "Wednesday": "středa",
    "Thursday": "čtvrtek",
    "Friday": "pátek",
    "Saturday": "sobota",
    "Sunday": "neděle"
  };

  transform(value: string): string {
    return this.names[value];
  }

}
