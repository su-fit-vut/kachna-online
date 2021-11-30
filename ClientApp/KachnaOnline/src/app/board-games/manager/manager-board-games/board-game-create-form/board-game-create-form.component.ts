// board-game-create-form.component.ts
// Author: František Nečas

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup } from "@angular/forms";
import { BoardGame } from "../../../../models/board-games/board-game.model";
import { ToastrService } from "ngx-toastr";
import { BoardGamesService } from "../../../../shared/services/board-games.service";
import { BoardGameCategory } from "../../../../models/board-games/board-game-category.model";

@Component({
  selector: 'app-board-game-create-form',
  templateUrl: './board-game-create-form.component.html',
  styleUrls: ['./board-game-create-form.component.css']
})
export class BoardGameCreateFormComponent implements OnInit {
  @Input() submitButtonText: string;
  // Current data for update.
  @Input() startingState: BoardGame | undefined = undefined;
  @Output() formSubmitted: EventEmitter<FormGroup> = new EventEmitter();
  categories: BoardGameCategory[] = []

  form = new FormGroup({
    name: new FormControl(''),
    description: new FormControl(''),
    image: new FormGroup({
      file: new FormControl(undefined),
    }),
    imageUrl: new FormControl(''),
    playersMin: new FormControl(undefined),
    playersMax: new FormControl(undefined),
    inStock: new FormControl(1),
    category: new FormControl(''),
    categoryId: new FormControl(undefined),
    noteInternal: new FormControl(''),
    ownerId: new FormControl(undefined),
    unavailable: new FormControl(0),
    visible: new FormControl(true),
    defaultReservationDays: new FormControl(undefined),
  })
  image: string = "";

  constructor(private toastrService: ToastrService, private boardGamesService: BoardGamesService) {
  }

  ngOnChanges(): void {
    if (this.startingState) {
      this.image = this.startingState?.imageUrl;
      this.form.patchValue({
        name: this.startingState.name,
        description: this.startingState.description,
        imageUrl: this.startingState.imageUrl,
        playersMin: this.startingState.playersMin,
        playersMax: this.startingState.playersMax,
        inStock: this.startingState.inStock,
        category: this.startingState.category.name,
        categoryId: this.startingState.category.id,
        noteInternal: this.startingState.noteInternal,
        unavailable: this.startingState.unavailable,
        visible: this.startingState.visible,
        defaultReservationDays: this.startingState.defaultReservationDays
      })
    }
  }

  ngOnInit(): void {
    this.boardGamesService.getCategories().subscribe(categories => {
      this.categories = categories;
      this.form.patchValue({category: this.startingState?.category});
      this.form.get('category')?.markAsDirty();
    }, err => {
      console.log(err);
      this.toastrService.error("Načtení kategorií selhalo.");
    })
  }

  onSubmit(): void {
    if (this.form.valid) {
      let value = this.form.value
      let min = value['min'];
      let max = value['max'];
      let categoryId = value['categoryId']
      if (!categoryId) {
        this.toastrService.error("Kategorie musí být zvolena.")
        return;
      }
      if (min && max && min > max) {
        this.toastrService.error("Minimální počet hráčů nesmí být vyšší než maximální.")
        return;
      }
      this.formSubmitted.emit(this.form);
    }
  }

  imageChanged(event: any): void {
    this.form.patchValue({image: {file: event.target.files.item(0)}});
  }

  categoryChanged(event: any): void {
    for (let category of this.categories) {
      if (category.name == event.target.value) {
        this.form.patchValue({categoryId: category.id});
        return
      }
    }
    this.form.patchValue({categoryId: undefined});
  }
}
