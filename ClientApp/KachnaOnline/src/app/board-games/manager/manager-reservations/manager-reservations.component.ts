// manager-reservations.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { Reservation, ReservationState } from "../../../models/board-games/reservation.model";
import { FormControl } from "@angular/forms";
import { BoardGamesService } from "../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { ActivatedRoute, Router } from "@angular/router";

@Component({
  selector: 'app-manager-reservations',
  templateUrl: './manager-reservations.component.html',
  styleUrls: ['./manager-reservations.component.css']
})
export class ManagerReservationsComponent implements OnInit {
  reservations: Reservation[]
  filterKeys: [string, ReservationState][] = [
    ["Nově přidané", ReservationState.New],
    ["Právě běžící", ReservationState.Current],
    ["Platnost vypršela", ReservationState.Expired],
    ["Dokončené", ReservationState.Done]
  ]
  reservationFilterForm = new FormControl("---");
  reservationFilter: ReservationState | undefined = undefined;
  assignedFilter: boolean = false;

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService,
              private router: Router, private activatedRoute: ActivatedRoute) {
  }

  ngOnInit(): void {
    [this.reservationFilter, this.assignedFilter] = this.boardGamesService.getManagerFilter();
    let formValue = this.filterKeys.find(k => k[1] == this.reservationFilter);
    this.reservationFilterForm.reset(formValue ? formValue[0] : "---");

    this.reservationFilterForm.valueChanges.subscribe(value => {
      let filter = this.filterKeys.find(k => k[0] == value);
      this.reservationFilter = filter ? filter[1] : undefined;
      this.fetchReservations();
    })
    this.fetchReservations();
  }

  ngOnDestroy(): void {
    this.boardGamesService.saveManagerFilter(this.reservationFilter, this.assignedFilter);
  }

  fetchReservations(): void {
    if (this.assignedFilter) {
      this.boardGamesService.getAllReservationsAssignedToMe(this.reservationFilter).subscribe(reservations => {
        this.reservations = reservations;
        this.sortReservations();
      }, err => {
        console.log(err);
        this.toastrService.error("Načtení všech tobě přiřazených rezervací selhalo.");
      });
    } else {
      this.boardGamesService.getAllReservations(this.reservationFilter).subscribe(reservations => {
        this.reservations = reservations;
        this.sortReservations();
      }, err => {
        console.log(err);
        this.toastrService.error("Načtení všech rezervací selhalo.");
      });
    }
  }

  onButtonToggle(newAssignedFilter: boolean): void {
    this.assignedFilter = newAssignedFilter;
    this.fetchReservations();
  }

  sortReservations(): void {
    this.reservations = this.reservations.sort((a, b) => {
      if (a.madeOn > b.madeOn) {
        return -1;
      } else if (a.madeOn < b.madeOn) {
        return 1;
      } else {
        return 0;
      }
    });
  }

  navigateToReservation(reservation: Reservation): void {
    this.router.navigate([`./${reservation.id}`], {relativeTo: this.activatedRoute}).then();
  }
}