import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Reservation } from "../../../models/board-games/reservation.model";
import { ReservationItemState } from "../../../models/board-games/reservation-item.model";

@Component({
  selector: 'app-reservation-table',
  templateUrl: './reservation-table.component.html',
  styleUrls: ['./reservation-table.component.css']
})
export class ReservationTableComponent implements OnInit {
  @Input() managerView: boolean = false;
  @Input() reservations: Reservation[] = [];
  @Output() reservationClicked: EventEmitter<Reservation> = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }

  onReservationClicked(reservation: Reservation): void {
    this.reservationClicked.emit(reservation);
  }

  isExpired(reservation: Reservation): boolean {
    return reservation.items.find(i => i.state == ReservationItemState.Expired) !== undefined;
  }

  isDone(reservation: Reservation): boolean {
    return reservation.items.every(
      i => i.state == ReservationItemState.Cancelled || i.state == ReservationItemState.Done);
  }

}
