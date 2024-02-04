import { Component, Output, OnInit, EventEmitter, Input } from '@angular/core';

@Component({
  selector: 'app-month-selection',
  templateUrl: './month-selection.component.html',
  styleUrls: ['./month-selection.component.css']
})
export class MonthSelectionComponent implements OnInit {

  @Input() public justifyCenter: boolean = true;
  @Input() public minMonth: Date;
  @Output() public monthChange: EventEmitter<Date> = new EventEmitter()

  month: Date;

  constructor() {
  }

  ngOnInit(): void {
    this.month = new Date();
  }

  changeMonth(delta: number) {
    this.month.setDate(1);
    this.month.setMonth(this.month.getMonth() + delta);
    this.monthChange.emit(this.month);
  }

  setDisabledIfMinMonthSet(): boolean {
    if (this.minMonth) {
      if (this.month.getFullYear() == this.minMonth.getFullYear() && this.month.getMonth() == this.minMonth.getMonth()) {
        return true;
      }
    }
    return false;
  }
}
