import { NgbDateStruct, NgbTimeStruct } from "@ng-bootstrap/ng-bootstrap";
import { NgbDateAdapter } from "@ng-bootstrap/ng-bootstrap/datepicker/adapters/ngb-date-adapter";

export class DateUtils {
  public static dateTimeToString(date: NgbDateStruct, time: NgbTimeStruct, adapter: NgbDateAdapter<Date>): string {
    let dateObj = new Date(date.year, date.month - 1, date.day, time.hour, time.minute, 0);
    return dateObj?.toISOString() ?? "";
  }
}
