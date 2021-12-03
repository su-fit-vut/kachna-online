// togglable-button.component.ts
// Author: František Nečas, Ondřej Ondryáš

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-togglable-button',
  templateUrl: './toggleable-button.component.html',
  styleUrls: ['./toggleable-button.component.css']
})
export class ToggleableButtonComponent implements OnInit {
  @Input() text: string = "";
  @Input() startingValue: boolean = false
  @Output() valueChanged: EventEmitter<boolean> = new EventEmitter()

  constructor() {
  }

  ngOnInit(): void {
  }

  onChange(isChecked: boolean) {
    this.valueChanged.emit(isChecked);
  }
}
