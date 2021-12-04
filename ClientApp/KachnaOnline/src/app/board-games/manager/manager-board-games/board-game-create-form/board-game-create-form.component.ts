// board-game-create-form.component.ts
// Author: František Nečas

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, Validators, FormBuilder, ValidationErrors, ValidatorFn, AbstractControl } from "@angular/forms";
import { BoardGame } from "../../../../models/board-games/board-game.model";
import { ToastrService } from "ngx-toastr";
import { BoardGamesService } from "../../../../shared/services/board-games.service";
import { BoardGameCategory } from "../../../../models/board-games/board-game-category.model";
import { UserDetail } from "../../../../models/users/user.model";

export const playersValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const min = control.get('playersMin');
  const max = control.get('playersMax');
  if (min === null || max === null || min.value === null || max.value === null || min.value <= max.value) {
    return null;
  } else {
    return {playersWrong: true};
  }
}

export const stockValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const stock = control.get('inStock');
  const unavailable = control.get('unavailable');
  return (stock && unavailable && stock.value >= unavailable.value) ? null : {stockWrong: true};
}

@Component({
  selector: 'app-board-game-create-form',
  templateUrl: './board-game-create-form.component.html',
  styleUrls: ['./board-game-create-form.component.css']
})
export class BoardGameCreateFormComponent implements OnInit {
  @Input() submitButtonText: string;
  // Current data for update.
  @Input() startingState: BoardGame;
  @Output() formSubmitted: EventEmitter<FormGroup> = new EventEmitter();
  categories: BoardGameCategory[] = []

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(256)]],
    description: [''],
    image: this.fb.group({
      file: [undefined]
    }),
    imageUrl: [''],
    playersMin: [undefined, Validators.min(1)],
    playersMax: [undefined, Validators.min(1)],
    inStock: [1, [Validators.required, Validators.min(0)]],
    category: ['', Validators.required],
    categoryId: [undefined, Validators.required],
    noteInternal: ['', Validators.maxLength(1024)],
    ownerId: [undefined],
    unavailable: [0, [Validators.required, Validators.min(0)]],
    visible: [true, Validators.required],
    defaultReservationDays: [undefined, Validators.min(1)],
  }, {validators: [playersValidator, stockValidator]})
  image: string = "";
  category: string = "";

  constructor(private toastrService: ToastrService, private boardGamesService: BoardGamesService,
              private fb: FormBuilder) {
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
      this.category = this.startingState.category.name;
    } else {
      this.category = "---";
    }
  }

  ngOnInit(): void {
    this.boardGamesService.getCategories().subscribe(categories => {
      this.categories = categories;
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

  userSelected(event: UserDetail): void {
    this.form.patchValue({ownerId: event.id});
  }

  categoryChanged(event: string): void {
    for (let category of this.categories) {
      if (category.name == event) {
        this.category = category.name;
        this.form.patchValue({categoryId: category.id, category: category.name});
        return
      }
    }
    this.form.patchValue({categoryId: undefined});
  }
}
