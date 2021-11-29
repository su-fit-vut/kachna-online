// manager-button-cluster.component.ts
// Author: František Nečas
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReservationItem, ReservationItemState } from "../../../../models/board-games/reservation-item.model";
import {
  ReservationEventType,
  ReservationItemEvent
} from "../../../../models/board-games/reservation-item-event.model";

@Component({
  selector: 'app-manager-button-cluster',
  templateUrl: './manager-button-cluster.component.html',
  styleUrls: ['./manager-button-cluster.component.css']
})
export class ManagerButtonClusterComponent implements OnInit {
  @Input() item: ReservationItem;
  @Output() cancel: EventEmitter<ReservationItem> = new EventEmitter();
  @Output() assign: EventEmitter<ReservationItem> = new EventEmitter();
  @Output() handOver: EventEmitter<ReservationItem> = new EventEmitter();
  @Output() extend: EventEmitter<ReservationItem> = new EventEmitter();
  @Output() refuseExtension: EventEmitter<ReservationItem> = new EventEmitter();
  @Output() returned: EventEmitter<ReservationItem> = new EventEmitter();

  constructor() { }

  public get itemState(): typeof ReservationItemState {
    return ReservationItemState;
  }

  public get eventType(): typeof ReservationEventType {
    return ReservationEventType;
  }

  ngOnInit(): void {
  }

  showHistory(): void {
    console.log(`Show history of item ${this.item.id}`);
  }
}
