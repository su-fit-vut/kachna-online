// reservation-details.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { BoardGamesService } from "../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpStatusCode } from "@angular/common/http";
import { Reservation } from "../../models/board-games/reservation-model";
import { FormControl } from "@angular/forms";
import { formatDate } from "@angular/common";
import { ReservationItem, ReservationItemState } from "../../models/board-games/reservation-item-model";

@Component({
  selector: 'app-reservation-details',
  templateUrl: './reservation-details.component.html',
  styleUrls: ['./reservation-details.component.css']
})
export class ReservationDetailsComponent implements OnInit {
  reservation: Reservation;
  items: ReservationItem[];
  noteForm: FormControl = new FormControl('');
  formattedDate: string;

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService,
              private router: Router, private route: ActivatedRoute) {
  }

  public get itemState(): typeof ReservationItemState {
    return ReservationItemState;
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.boardGamesService.getReservation(params['id']).subscribe(reservation => {
          this.reservation = reservation;
          this.items = reservation.items;
          this.noteForm.setValue(this.reservation.noteUser);
          this.formattedDate = formatDate(this.reservation.madeOn, "d. M. y", "cs-CZ");
        },
        err => {
          console.log(err);
          if (err.status == HttpStatusCode.Unauthorized || err.status == HttpStatusCode.Forbidden) {
            this.toastrService.error("Zdá se, že tato rezervace není tvoje nebo nejsi přihlášen*a.");
          } else {
            this.toastrService.error("Načtení rezervace se nezdařilo.")
          }
          this.router.navigate(['..'], {relativeTo: this.route}).then()

        })
    })
  }

  updateNote(): void {
    this.boardGamesService.setReservationUserNote(this.reservation.id, this.noteForm.value).subscribe(_ => {
        this.toastrService.success("Poznámka uložena.");
      },
      err => {
        console.log(err);
        this.toastrService.error("Nastavení poznámky selhalo (možná je příliš dlouhá?).");
      })
  }
}
