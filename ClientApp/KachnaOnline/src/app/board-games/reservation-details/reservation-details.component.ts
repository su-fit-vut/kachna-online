import { Component, Input, OnInit } from '@angular/core';
import { BoardGamesService } from "../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpStatusCode } from "@angular/common/http";
import { Reservation } from "../../models/board-games/reservation.model";
import { FormControl } from "@angular/forms";
import { formatDate } from "@angular/common";
import { ReservationItem } from "../../models/board-games/reservation-item.model";
import { BoardGamesStoreService } from "../../shared/services/board-games-store.service";
import { Subscription } from "rxjs";

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
  reservationId: number;
  routeSub: Subscription;

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService,
              private router: Router, private route: ActivatedRoute, private storeService: BoardGamesStoreService) {
  }

  ngOnInit(): void {
    this.routeSub = this.route.params.subscribe(params => {
      this.reservationId = params['id'];
      this.fetchReservation();
    })
  }

  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
  }

  fetchReservation(): void {
    this.boardGamesService.getReservation(this.reservationId).subscribe(reservation => {
        this.reservation = reservation;
        this.items = reservation.items;
        this.noteForm.setValue(this.reservation.noteUser);
        this.formattedDate = formatDate(this.reservation.madeOn, "d. M. y", "cs-CZ");
      },
      err => {
        console.log(err);
        if (err.status == HttpStatusCode.Unauthorized || err.status == HttpStatusCode.Forbidden) {
          this.toastrService.error("Zd?? se, ??e tato rezervace nen?? tvoje nebo nejsi p??ihl????en*a.");
        } else {
          this.toastrService.error("Na??ten?? rezervace se nezda??ilo.")
        }
        this.router.navigate(['..'], {relativeTo: this.route}).then()

      })
  }

  fetchItem(item: ReservationItem): void {
    this.boardGamesService.getReservationItem(this.reservationId, item.id).subscribe(item => {
      for (let i = 0; i < this.items.length; i++) {
        if (this.items[i].id == item.id) {
          this.items[i] = item;
          break;
        }
      }
    }, err => {
      console.log(err);
      this.toastrService.error("Nepoda??ilo se znovu na????st p??edm??t v rezervaci.");
    });
  }

  updateNote(): void {
    this.boardGamesService.setReservationUserNote(this.reservation.id, this.noteForm.value).subscribe(_ => {
        this.toastrService.success("Pozn??mka ulo??ena.");
      },
      err => {
        console.log(err);
        this.toastrService.error("Nastaven?? pozn??mky selhalo (mo??n?? je p????li?? dlouh???).");
      })
  }

  routeToBoardGame(item: ReservationItem): void {
    this.storeService.saveBackRoute(this.router.url);
    this.router.navigate([`/board-games/${item.boardGame.id}`]).then();
  }
}
