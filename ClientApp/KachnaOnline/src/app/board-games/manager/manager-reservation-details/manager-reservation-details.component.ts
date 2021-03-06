import { Component, OnInit } from '@angular/core';
import { BoardGamesService } from "../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { ActivatedRoute, Router } from "@angular/router";
import { Reservation } from "../../../models/board-games/reservation.model";
import { ReservationItem, ReservationItemState } from "../../../models/board-games/reservation-item.model";
import { FormControl } from "@angular/forms";
import { formatDate } from "@angular/common";
import { ReservationEventType } from "../../../models/board-games/reservation-item-event.model";
import { BoardGamePageState, BoardGamesStoreService } from "../../../shared/services/board-games-store.service";
import { Subscription } from "rxjs";

@Component({
  selector: 'app-manager-reservation-details',
  templateUrl: './manager-reservation-details.component.html',
  styleUrls: ['./manager-reservation-details.component.css']
})
export class ManagerReservationDetailsComponent implements OnInit {
  reservation: Reservation;
  items: ReservationItem[] = [];
  noteInternalForm: FormControl = new FormControl('');
  formattedDate: string;
  reservationId: number;
  routeSub: Subscription

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
        this.noteInternalForm.setValue(this.reservation.noteInternal);
        this.formattedDate = formatDate(this.reservation.madeOn, "d. M. y", "cs-CZ");
      },
      err => {
        console.log(err);
        this.toastrService.error("Na??ten?? rezervace se nezda??ilo. Jsi p??ihl????en*a?")
        this.router.navigate(['..'], {relativeTo: this.route}).then()
      })
  }

  fetchItem(item: ReservationItem): void {
    this.boardGamesService.getReservationItem(this.reservationId, item.id).subscribe(item => {
      for (let i = 0; i < this.items.length; i++) {
        if (this.items[i].id == item.id) {
          this.items[i] = item;
        }
      }
    }, err => {
      console.log(err);
      this.toastrService.error("Aktualizace informac?? o ????sti rezervace selhala.")
    })
  }

  routeToBoardGame(item: ReservationItem): void {
    this.storeService.saveBackRoute(this.router.url);
    this.router.navigate([`/board-games/${item.boardGame.id}`]).then();
  }

  updateNote(): void {
    this.boardGamesService.setReservationInternalNote(this.reservation.id, this.noteInternalForm.value).subscribe(_ => {
        this.toastrService.success("Pozn??mka ulo??ena.");
      },
      err => {
        console.log(err);
        this.toastrService.error("Nastaven?? pozn??mky selhalo (mo??n?? je p????li?? dlouh???).");
      })
  }

  addGames() {
    this.storeService.setPageMode(BoardGamePageState.AddingGames);
    this.storeService.saveReservationId(this.reservationId);
    this.router.navigate(['/board-games']).then();
  }

  canBeAssigned(): boolean {
    return this.items.find(i => !i.assignedTo && i.state != ReservationItemState.Cancelled) !== undefined;
  }

  canBeHandedOver(): boolean {
    return this.items.find(i => i.state == ReservationItemState.Assigned) !== undefined;
  }

  canBeReturned(): boolean {
    return this.items.find(
      i => i.state == ReservationItemState.HandedOver || i.state == ReservationItemState.Expired) !== undefined;
  }

  canBeExtended(): boolean {
    return this.items.every(i => i.state == ReservationItemState.Expired);
  }

  assignAll(): void {
    for (let item of this.items) {
      if (!item.assignedTo && item.state == ReservationItemState.New) {
        this.boardGamesService.updateReservationState(this.reservationId, item.id, ReservationEventType.Assigned).subscribe(_ => {
          this.fetchItem(item);
        }, err => {
          console.log(err);
          this.toastrService.error("Nepoda??ilo se p??i??adit n??kterou z her.");
        })
      }
    }
  }

  handOverAll(): void {
    for (let item of this.items) {
      if (item.state == ReservationItemState.Assigned) {
        this.boardGamesService.updateReservationState(this.reservationId, item.id, ReservationEventType.HandedOver).subscribe(_ => {
          this.fetchItem(item);
        }, err => {
          console.log(err);
          this.toastrService.error("Nepoda??ilo se p??edat n??kterou z her.");
        })
      }
    }
  }

  returnAll(): void {
    for (let item of this.items) {
      if (item.state == ReservationItemState.HandedOver || item.state == ReservationItemState.Expired) {
        this.boardGamesService.updateReservationState(this.reservationId, item.id, ReservationEventType.Returned).subscribe(_ => {
          this.fetchItem(item);
        }, err => {
          console.log(err);
          this.toastrService.error("Nepoda??ilo se navr??tit n??kterou z her.")
        })
      }
    }
  }

  extendAll(): void {
    for (let item of this.items) {
      this.boardGamesService.updateReservationState(this.reservationId, item.id, ReservationEventType.ExtensionGranted).subscribe(_ => {
        this.fetchItem(item);
      }, err => {
        console.log(err);
        this.toastrService.error("Nepoda??ilo se prodlou??it n??kterou z her.")
      })
    }
  }
}
