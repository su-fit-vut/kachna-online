// local-date-parser-formatter.ts
// Author: Ondřej Ondryáš

import { Injectable } from "@angular/core";
import { NgbDateParserFormatter, NgbDateStruct } from "@ng-bootstrap/ng-bootstrap";

@Injectable()
export class CustomDateParserFormatter extends NgbDateParserFormatter {

  readonly DELIMITER = ".";
  readonly DELIMITER_SPACE = ". ";

  parse(value: string): NgbDateStruct | null {
    if (value) {
      let date = value.split(this.DELIMITER);
      return {
        day: parseInt(date[0].trim(), 10),
        month: parseInt(date[1].trim(), 10),
        year: parseInt(date[2].trim(), 10)
      };
    }
    return null;
  }

  format(date: NgbDateStruct | null): string {
    return date ? date.day + this.DELIMITER_SPACE + date.month + this.DELIMITER_SPACE + date.year : '';
  }
}
