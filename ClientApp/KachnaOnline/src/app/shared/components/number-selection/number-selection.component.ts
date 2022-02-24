import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl } from "@angular/forms";

@Component({
  selector: 'app-number-selection',
  templateUrl: './number-selection.component.html',
  styleUrls: ['./number-selection.component.css']
})
export class NumberSelectionComponent implements OnInit {
  @Input() placeholder: string;
  @Input() minimum: number | undefined;
  @Input() maximum: number | undefined;
  @Input() initialValue: number | undefined = undefined;
  @Output() valueChanged: EventEmitter<number | undefined> = new EventEmitter();
  countForm = new FormControl(undefined);

  constructor() {
  }

  ngOnChanges(): void {
    this.countForm.setValue(this.initialValue);
  }

  ngOnInit(): void {
    this.countForm.setValue(this.initialValue);
    this.countForm.valueChanges.subscribe(value => {
      if (value !== undefined && this.minimum !== undefined && value < this.minimum) {
        this.countForm.setValue(this.minimum);
        return;
      }
      if (value !== undefined && this.maximum !== undefined && value > this.maximum) {
        this.countForm.setValue(this.maximum);
        return;
      }
      this.valueChanged.emit(value);
    })
  }

  increment(): void {
    let newValue: number;
    if (this.countForm.value) {
      newValue = this.countForm.value + 1;
    } else if (this.minimum) {
      newValue = this.minimum;
    } else {
      newValue = 0;
    }
    this.countForm.setValue(newValue);
  }

  decrement(): void {
    let newValue: number;
    if (this.countForm.value) {
      newValue = this.countForm.value -1;
    } else if (this.maximum) {
      newValue = this.maximum;
    } else {
      newValue = 0;
    }
    this.countForm.setValue(newValue);
  }

  reset(): void {
    this.countForm.setValue(undefined);
  }
}
