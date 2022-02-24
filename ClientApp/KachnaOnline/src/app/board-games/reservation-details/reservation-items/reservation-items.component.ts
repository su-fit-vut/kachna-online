import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReservationItem, ReservationItemState } from "../../../models/board-games/reservation-item.model";
import { Reservation } from "../../../models/board-games/reservation.model";
import { ReservationEventType } from "../../../models/board-games/reservation-item-event.model";

@Component({
  selector: 'app-reservation-items',
  templateUrl: './reservation-items.component.html',
  styleUrls: ['./reservation-items.component.css']
})
export class ReservationItemsComponent implements OnInit {
  @Input() reservation: Reservation;
  @Input() items: ReservationItem[];
  @Input() managerView: boolean = false;
  @Output() reservationItemClicked: EventEmitter<ReservationItem> = new EventEmitter();
  @Output() itemStateUpdated: EventEmitter<ReservationItem> = new EventEmitter();

  constructor() {
  }

  public get eventType(): typeof ReservationEventType {
    return ReservationEventType;
  }

  public get itemState(): typeof ReservationItemState {
    return ReservationItemState;
  }

  ngOnInit(): void {
  }

  rowClicked(item: ReservationItem): void {
    this.reservationItemClicked.emit(item);
  }
}
