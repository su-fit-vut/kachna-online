import { Component, Output, OnInit, EventEmitter, Input } from '@angular/core';

@Component({
  selector: 'app-year-selection',
  templateUrl: './year-selection.component.html',
})
export class YearSelectionComponent implements OnInit {
  @Input() public justifyCenter: boolean = true;
  @Output() public yearChange: EventEmitter<Date> = new EventEmitter()

  year: Date;

  constructor() {
  }

  ngOnInit(): void {
    this.year = new Date();
  }

  changeYear(delta: number) {
    this.year.setFullYear(this.year.getFullYear() + delta);
    this.yearChange.emit(this.year);
  }
}
