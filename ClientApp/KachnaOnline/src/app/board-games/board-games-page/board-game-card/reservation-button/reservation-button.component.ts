// reservation-button.component.ts
// Author: František Nečas

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AuthenticationService } from "../../../../shared/services/authentication.service";

@Component({
  selector: 'app-reservation-button',
  templateUrl: './reservation-button.component.html',
  styleUrls: ['./reservation-button.component.css']
})
export class ReservationButtonComponent implements OnInit {
  // Allow setting z-index of the button
  @Input() styles: any = {}
  @Input() available: number
  @Input() currentValue: number = 0
  @Output() countChanged: EventEmitter<number> = new EventEmitter()

  constructor(public authenticationService: AuthenticationService) {
  }

  ngOnInit(): void {
  }

  reservedInitial(): void {
    this.currentValue = 1;
    this.countChanged.emit(this.currentValue);
  }

  reservedIncrement(): void {
    this.currentValue++;
    this.countChanged.emit(this.currentValue);
  }

  reservedDecrement(): void {
    this.currentValue--;
    this.countChanged.emit(this.currentValue);
  }
}
