// category-update.component.ts
// Author: František Nečas

import { Component, Input, OnInit } from '@angular/core';
import { BoardGameCategory } from "../../../../models/board-games/board-game-category.model";
import { BoardGamesService } from "../../../../shared/services/board-games.service";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { FormControl, FormGroup } from "@angular/forms";
import { HttpStatusCode } from "@angular/common/http";
import { Subscription } from "rxjs";

@Component({
  selector: 'app-category-update',
  templateUrl: './category-update.component.html',
  styleUrls: ['./category-update.component.css']
})
export class CategoryUpdateComponent implements OnInit {
  categoryForm = new FormGroup({
    name: new FormControl(''),
    color: new FormControl('#000000')
  })
  color: string = '#000000';
  category: BoardGameCategory;
  routeSub: Subscription;

  constructor(private boardGamesService: BoardGamesService, private router: Router,
              private toastrService: ToastrService, private activatedRoute: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.routeSub = this.activatedRoute.params.subscribe(data => {
      this.fetchCategory(data['id']);
    })
  }

  ngOnDestroy(): void {
    this.routeSub.unsubscribe();
  }

  fetchCategory(id: number): void {
    this.boardGamesService.getCategory(id).subscribe(category => {
      this.category = category;
      this.color = '#' + this.category.colourHex;
      this.categoryForm.patchValue({name: this.category.name, color: this.color});
    }, err => {
      console.log(err);
      this.toastrService.error("Načtení kategorie se nezdařilo.");
      this.router.navigate(['..'], {relativeTo: this.activatedRoute}).then();
    })
  }

  onSubmit(): void {
    let value = this.categoryForm.value
    this.boardGamesService.updateCategory(this.category.id, value['name'], value['color']).subscribe(_ => {
      this.toastrService.success("Kategorie upravena.");
      this.fetchCategory(this.category.id);
    }, err => {
      console.log(err);
      this.toastrService.error("Úprava kategorie selhala. Je barva ve formátu #000000?");
    })
  }

  onColorChange(color: string): void {
    this.color = color;
    this.categoryForm.patchValue({color: color});
  }

  deleteCategory(): void {
    this.boardGamesService.deleteCategory(this.category.id).subscribe(_ => {
      this.toastrService.success("Kategorie smazána.");
      this.router.navigate(['..'], {relativeTo: this.activatedRoute}).then();
    }, err => {
      console.log(err);
      if (err.status == HttpStatusCode.Conflict) {
        this.toastrService.error("Kategorii nejde smazat, protože obsahuje deskové hry. Nejdříve je " +
          "nutné tyto hry přesunout do jiné kategorie.");
      } else {
        this.toastrService.error("Odstranění kategorie se nezdařilo");
      }
    })
  }

}
