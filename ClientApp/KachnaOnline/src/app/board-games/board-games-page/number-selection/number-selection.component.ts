// number-selection.component.ts
// Author: František Nečas

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

  ngOnInit(): void {
    console.log(`Setting to ${this.initialValue}`)
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
    this.countForm.setValue((this.countForm.value || this.minimum || 0) + 1);
  }

  decrement(): void {
    this.countForm.setValue((this.countForm.value || this.maximum || 0) - 1);
  }

  reset(): void {
    this.countForm.setValue(undefined);
  }
}
