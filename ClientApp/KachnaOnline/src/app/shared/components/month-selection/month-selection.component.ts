import { Component, Output, OnInit, EventEmitter, Input } from '@angular/core';

@Component({
  selector: 'app-month-selection',
  templateUrl: './month-selection.component.html',
  styleUrls: ['./month-selection.component.css']
})
export class MonthSelectionComponent implements OnInit {

  @Input() public justifyCenter: boolean = true;
  @Output() public monthChange: EventEmitter<Date> = new EventEmitter()

  month: Date;

  constructor() {
  }

  ngOnInit(): void {
    this.month = new Date();
  }

  changeMonth(delta: number) {
    this.month.setMonth(this.month.getMonth() + delta);
    this.monthChange.emit(this.month);
  }
}
