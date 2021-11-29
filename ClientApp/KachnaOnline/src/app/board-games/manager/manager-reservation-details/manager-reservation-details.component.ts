// manager-reservation-details.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { BoardGamesService } from "../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { ActivatedRoute, Router } from "@angular/router";
import { Reservation } from "../../../models/board-games/reservation.model";
import { ReservationItem } from "../../../models/board-games/reservation-item.model";
import { FormControl } from "@angular/forms";
import { formatDate } from "@angular/common";
import { HttpStatusCode } from "@angular/common/http";

@Component({
  selector: 'app-manager-reservation-details',
  templateUrl: './manager-reservation-details.component.html',
  styleUrls: ['./manager-reservation-details.component.css']
})
export class ManagerReservationDetailsComponent implements OnInit {
  reservation: Reservation;
  items: ReservationItem[];
  noteInternalForm: FormControl = new FormControl('');
  formattedDate: string;
  reservationId: number;

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService,
              private router: Router, private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.reservationId = params['id'];
      this.fetchReservation();
    })
  }

  fetchReservation(): void {
    this.boardGamesService.getReservation(this.reservationId).subscribe(reservation => {
        this.reservation = reservation;
        this.items = reservation.items;
        this.noteInternalForm.setValue(this.reservation.noteUser);
        this.formattedDate = formatDate(this.reservation.madeOn, "d. M. y", "cs-CZ");
      },
      err => {
        console.log(err);
        this.toastrService.error("Načtení rezervace se nezdařilo.")
        this.router.navigate(['..'], {relativeTo: this.route}).then()

      })

  }


  routeToBoardGame(item: ReservationItem): void {
    this.boardGamesService.saveBackRoute(this.router.url);
    this.router.navigate([`/board-games/${item.boardGame.id}`]).then();
  }

  updateNote(): void {
    this.boardGamesService.setReservationInternalNote(this.reservation.id, this.noteInternalForm.value).subscribe(_ => {
        this.toastrService.success("Poznámka uložena.");
      },
      err => {
        console.log(err);
        this.toastrService.error("Nastavení poznámky selhalo (možná je příliš dlouhá?).");
      })
  }
}
