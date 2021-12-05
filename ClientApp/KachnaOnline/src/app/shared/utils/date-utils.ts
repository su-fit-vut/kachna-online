// date-utils.component.ts
// Author: Ondřej Ondryáš

import { NgbDateStruct, NgbTimeStruct } from "@ng-bootstrap/ng-bootstrap";
import { NgbDateAdapter } from "@ng-bootstrap/ng-bootstrap/datepicker/adapters/ngb-date-adapter";

export class DateUtils {
  public static dateTimeToString(date: NgbDateStruct, time: NgbTimeStruct, adapter: NgbDateAdapter<Date>): string {
    let dateObj = adapter.toModel(date);

    dateObj?.setHours(time.hour);
    dateObj?.setMinutes(time.minute);
    dateObj?.setSeconds(0);
    dateObj?.setTime(dateObj?.getTime() - dateObj?.getTimezoneOffset() * 60000);

    return dateObj?.toISOString() ?? "";
  }
}
