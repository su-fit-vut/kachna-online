// reservation-history.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { ReservationEventType, ReservationItemEvent } from "../../../models/board-games/reservation-item-event.model";
import { BoardGamesService } from "../../../shared/services/board-games.service";
import { ActivatedRoute } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { formatDate } from "@angular/common";

@Component({
  selector: 'app-reservation-history',
  templateUrl: './reservation-history.component.html',
  styleUrls: ['./reservation-history.component.css']
})
export class ReservationHistoryComponent implements OnInit {
  formattedEventType: Map<ReservationEventType, string> = new Map([
    [ReservationEventType.Created, "Vytvoření rezervace"],
    [ReservationEventType.Assigned, "Přiřazení rezervace"],
    [ReservationEventType.HandedOver, "Předání hry"],
    [ReservationEventType.Cancelled, "Zrušení rezervace"],
    [ReservationEventType.ExtensionRequested, "Žádost o prodloužení"],
    [ReservationEventType.ExtensionRefused, "Prodloužení zamítnuto"],
    [ReservationEventType.ExtensionGranted, "Prodlouženo"],
    [ReservationEventType.Returned, "Převzato zpět"]
  ])
  events: ReservationItemEvent[] = [];

  constructor(private boardGamesService: BoardGamesService, private route: ActivatedRoute,
              private toastrService: ToastrService) { }

  public get eventType(): typeof ReservationEventType {
    return ReservationEventType;
  }

  dateSeconds(date: Date): string {
    return formatDate(date, "d. M. y HH:MM:ss", "cs-CZ");
  }

  dateDays(date: Date): string {
    return formatDate(date, "d. M. y", "cs-CZ");
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.boardGamesService.getReservationItemHistory(params['id'], params['itemId']).subscribe(events => {
        this.events = events;
      }, err => {
        console.log(err);
        this.toastrService.error("Načtení historie rezervace selhalo.");
      })
    })
  }

}
