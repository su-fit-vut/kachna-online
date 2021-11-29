// category-create.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { BoardGamesService } from "../../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { ActivatedRoute, Router } from "@angular/router";
import { FormControl, FormGroup } from "@angular/forms";

@Component({
  selector: 'app-category-create',
  templateUrl: './category-create.component.html',
  styleUrls: ['./category-create.component.css']
})
export class CategoryCreateComponent implements OnInit {
  categoryForm = new FormGroup({
    name: new FormControl(''),
    color: new FormControl('#000000')
  })
  color: string = '#000000';

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService,
              private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
  }

  onSubmit(): void {
    let value = this.categoryForm.value
    this.boardGamesService.createCategory(value['name'], value['color']).subscribe(category => {
      this.toastrService.success("Kategorie vytvořena.");
      this.router.navigate([`../${category.id}`], {relativeTo: this.activatedRoute}).then();
    }, err => {
      console.log(err);
      this.toastrService.error("Přidání kategorie se nezdařila.");
    })
  }

  onColorChange(color: string): void {
    this.color = color;
    this.categoryForm.patchValue({color: color});
  }
}
