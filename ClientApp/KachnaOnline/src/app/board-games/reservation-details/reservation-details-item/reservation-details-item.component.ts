// reservation-details-item.component.ts
// Author: František Nečas

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReservationItem, ReservationItemState } from "../../../models/board-games/reservation-item.model";
import { BoardGamesService } from "../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { formatDate } from "@angular/common";
import { ReservationEventType } from "../../../models/board-games/reservation-item-event.model";
import { HttpStatusCode } from "@angular/common/http";

@Component({
  selector: '[app-reservation-details-item]',
  templateUrl: './reservation-details-item.component.html',
  styleUrls: ['./reservation-details-item.component.css']
})
export class ReservationDetailsItemComponent implements OnInit {
  @Input() reservationId: number;
  @Input() item: ReservationItem;
  @Output() reservationItemClicked: EventEmitter<ReservationItem> = new EventEmitter();
  formattedExpiration: string;
  formattedStates: Map<string, string> = new Map([
    ["New", "Nová"],
    ["Cancelled", "Zrušená"],
    ["Assigned", "Připravena k předání"],
    ["HandedOver", "Převzatá"],
    ["Done", "Vrácená"],
    ["Expired", "Platnost vypršela"]
  ]);

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService) {
  }

  public get itemState(): typeof ReservationItemState {
    return ReservationItemState;
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
        this.item.state = ReservationItemState.Cancelled;
        this.toastrService.success("Rezervace hry zrušena.");
      },
      err => {
        console.log(err);
        this.toastrService.error("Zrušení se nezdařilo.");
      }
    )
  }

  requestExtension(): void {
    this.boardGamesService.updateReservationState(this.reservationId, this.item.id,
      ReservationEventType.ExtensionRequested).subscribe(_ => {
      this.toastrService.success("Bylo zažádáno o prodloužení. Vyčkej, prosím, než ho někdo schválí " +
        "nebo napiš kontaktní osobě.");
    }, err => {
        console.log(err);
        if (err.status == HttpStatusCode.Conflict) {
          this.toastrService.warning("Již jsi žádal*a o prodloužení, vyčkej, prosím, než ho někdo schválí " +
            "nebo napiš kontaktní osobě.");
        } else {
          this.toastrService.error("Zaslání žádosti o prodloužení selhalo.");
        }
    })
  }
}
