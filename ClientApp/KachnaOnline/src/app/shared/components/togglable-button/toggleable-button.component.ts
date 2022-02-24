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

  id: string;

  constructor() {
    this.id = "toggle_" + Math.floor(Math.random() * 1000);
  }

  ngOnInit(): void {
  }

  onChange(isChecked: boolean) {
    this.valueChanged.emit(isChecked);
  }
}
