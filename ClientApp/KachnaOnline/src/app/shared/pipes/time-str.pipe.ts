import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeStr'
})
export class TimeStrPipe implements PipeTransform {

  transform(value: string): string {
    let valueParts = value.split(':');
    if (valueParts.length < 2) {
      return "??:??";
    }

    return valueParts[0] + ":" + valueParts[1];
  }

}
