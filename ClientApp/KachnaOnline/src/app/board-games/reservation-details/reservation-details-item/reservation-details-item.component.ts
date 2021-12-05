// reservation-details-item.component.ts
// Author: František Nečas

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReservationItem, ReservationItemState } from "../../../models/board-games/reservation-item.model";
import { BoardGamesService } from "../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { formatDate } from "@angular/common";
import { ReservationEventType, ReservationItemEvent } from "../../../models/board-games/reservation-item-event.model";
import { HttpStatusCode } from "@angular/common/http";

@Component({
  selector: '[app-reservation-details-item]',
  templateUrl: './reservation-details-item.component.html',
  styleUrls: ['./reservation-details-item.component.css']
})
export class ReservationDetailsItemComponent implements OnInit {
  @Input() reservationId: number;
  @Input() item: ReservationItem;
  @Input() managerView: boolean = false;
  @Output() reservationItemClicked: EventEmitter<ReservationItem> = new EventEmitter();
  @Output() stateChanged: EventEmitter<ReservationItem> = new EventEmitter();

  formattedExpiration: string;
  formattedStates: Map<string, string> = new Map([
    ["New", "Nová"],
    ["Cancelled", "Zrušená"],
    ["Assigned", "Připravena k předání"],
    ["HandedOver", "Převzatá"],
    ["Done", "Vrácená"],
    ["Expired", "Výpůjční doba vypršela"]
  ]);

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService) {
  }

  public get itemState(): typeof ReservationItemState {
    return ReservationItemState;
  }

  public get eventType(): typeof ReservationEventType {
    return ReservationEventType;
  }

  ngOnInit(): void {
    if (!this.item.expiresOn) {
      this.formattedExpiration = "";
    } else {
      this.formattedExpiration = formatDate(this.item.expiresOn, "d. M. y", "cs-CZ");
    }
  }

  cancelReservation(): void {
    this.boardGamesService.updateReservationState(this.reservationId, this.item.id,
      ReservationEventType.Cancelled).subscribe(_ => {
        this.stateChanged.emit(this.item);
        this.toastrService.success("Rezervace hry zrušena.");
      },
      err => {
        console.log(err);
        this.toastrService.error("Zrušení rezervace se nezdařilo. Zkus obnovit stránku, zda tvoji " +
          "rezervaci již nezačal řešit správce.");
      }
    )
  }

  requestExtension(): void {
    this.boardGamesService.updateReservationState(this.reservationId, this.item.id,
      ReservationEventType.ExtensionRequested).subscribe(_ => {
      this.stateChanged.emit(this.item);
      this.toastrService.success("Bylo zažádáno o prodloužení. Vyčkej, prosím, než ho někdo schválí, " +
        "nebo napiš kontaktní osobě.");
    }, err => {
      console.log(err);
      if (err.status == HttpStatusCode.Conflict) {
        this.toastrService.warning("Již jsi žádal*a o prodloužení, vyčkej, prosím, než ho někdo schválí, " +
          "nebo napiš kontaktní osobě.");
      } else {
        this.toastrService.error("Zaslání žádosti o prodloužení selhalo.");
      }
    })
  }

  handOver(): void {
    this.boardGamesService.updateReservationState(this.reservationId, this.item.id,
      ReservationEventType.HandedOver).subscribe(_ => {
        this.stateChanged.emit(this.item);
        this.toastrService.success("Hra byla předána.");
      },
      err => {
        console.log(err);
        this.toastrService.error("Předání selhalo.");
      }
    )
  }

  extend(): void {
    this.boardGamesService.updateReservationState(this.reservationId, this.item.id,
      ReservationEventType.ExtensionGranted).subscribe(_ => {
        this.stateChanged.emit(this.item);
        this.toastrService.success("Rezervace byla prodloužena.");
      },
      err => {
        console.log(err);
        this.toastrService.error("Prodloužení selhalo.");
      }
    )
  }

  refuseExtension(): void {
    this.boardGamesService.updateReservationState(this.reservationId, this.item.id,
      ReservationEventType.ExtensionRefused).subscribe(_ => {
        this.stateChanged.emit(this.item);
        this.toastrService.success("Prodloužení bylo zamítnuto.");
      },
      err => {
        console.log(err);
        this.toastrService.error("Zamítnutí prodloužení selhalo.");
      }
    )
  }

  return(): void {
    this.boardGamesService.updateReservationState(this.reservationId, this.item.id,
      ReservationEventType.Returned).subscribe(_ => {
        this.item.state = ReservationItemState.Done;
        this.toastrService.success("Hra byla úspěšně navrácena.");
      },
      err => {
        console.log(err);
        this.toastrService.error("Navrácení hry se nezdařilo.");
      }
    )
  }
}
