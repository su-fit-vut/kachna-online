// board-game-create.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { FormGroup } from "@angular/forms";
import { BoardGamesService } from "../../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { ImageUploadService } from "../../../../shared/services/image-upload.service";
import { HttpStatusCode } from "@angular/common/http";
import { ActivatedRoute, Router } from "@angular/router";

@Component({
  selector: 'app-board-game-create',
  templateUrl: './board-game-create.component.html',
  styleUrls: ['./board-game-create.component.css']
})
export class BoardGameCreateComponent implements OnInit {
  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService,
              private imageUploadService: ImageUploadService, private router: Router,
              private activatedRouter: ActivatedRoute) { }

  ngOnInit(): void {
  }

  postGame(data: object): void {
    this.boardGamesService.createBoardGame(data).subscribe(game => {
      this.toastrService.success("Hra byla přidána.")
      this.router.navigate([`../${game.id}`], {relativeTo: this.activatedRouter}).then();
    }, err => {
      console.log(err);
      if (err.status == HttpStatusCode.UnprocessableEntity) {
        this.toastrService.error("Přidání deskové hry selhalo. Minimální počet hráčů nesmí být vyšší než " +
          "maximální.")
      }
      this.toastrService.error("Přidání deskové hry selhalo. Nechybí ve formuláři nějaká informace?");
    })
  }

  createGame(form: FormGroup): void {
    // Process image and create
    let image = form.value['image'];
    delete form.value['category'];
    delete form.value['image'];
    if (image && image.file) {
      this.imageUploadService.postFile(image['file']).subscribe(data => {
        form.patchValue({imageUrl: data.url});
        this.postGame(form.value);
      }, err => {
        if (err.status == HttpStatusCode.Conflict) {
          form.patchValue({imageUrl: err.error.url});
          this.postGame(form.value);
        } else {
          this.toastrService.error("Nepodařilo se nahrát obrázek na server.");
        }
      })
    } else {
      form.patchValue({imageUrl: null});
      this.postGame(form.value)
    }
  }
}
