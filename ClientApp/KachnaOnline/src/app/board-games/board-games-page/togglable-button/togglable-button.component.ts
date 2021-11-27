// togglable-button.component.ts
// Author: František Nečas

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-togglable-button',
  templateUrl: './togglable-button.component.html',
  styleUrls: ['./togglable-button.component.css']
})
export class TogglableButtonComponent implements OnInit {
  @Input() text: string = "";
  @Input() startingValue: boolean = false
  @Output() valueChanged: EventEmitter<boolean> = new EventEmitter()

  constructor() { }

  ngOnInit(): void {
  }

  toggleValue() {
    this.startingValue = !this.startingValue;
    this.valueChanged.emit(this.startingValue);
  }
}
