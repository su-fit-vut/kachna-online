import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeLocative'
})
export class TimeLocativePipe implements PipeTransform {

  transform(value: Date): string {
    let hours = value.getHours();
    let isLong = [2, 3, 4, 12, 14, 20, 21, 22, 23].includes(hours);
    return (isLong ? "ve " : "v ") + value.toLocaleTimeString("cs-cz", {hour: "numeric", minute: "numeric"});
  }

}
