// board-game-update.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { BoardGamesService } from "../../../../shared/services/board-games.service";
import { ToastrService } from "ngx-toastr";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { BoardGame } from "../../../../models/board-games/board-game.model";
import { FormGroup } from "@angular/forms";
import { HttpStatusCode } from "@angular/common/http";
import { ImageUploadService } from "../../../../shared/services/image-upload.service";

@Component({
  selector: 'app-board-game-update',
  templateUrl: './board-game-update.component.html',
  styleUrls: ['./board-game-update.component.css']
})
export class BoardGameUpdateComponent implements OnInit {
  routeSub: Subscription
  game: BoardGame

  constructor(private boardGamesService: BoardGamesService, private toastrService: ToastrService,
              private activatedRoute: ActivatedRoute, private imageUploadService: ImageUploadService) { }

  ngOnInit(): void {
    this.routeSub = this.activatedRoute.params.subscribe(params => {
      this.fetchGame(params['id']);
    })
  }

  fetchGame(id: number): void {
    this.boardGamesService.getBoardGame(id).subscribe(game => {
      this.game = game;
    }, err => {
      console.log(err);
      this.toastrService.error("Nepodařilo se načíst hru.");
    })
  }

  putGame(data: object): void {
    this.boardGamesService.updateBoardGame(this.game.id, data).subscribe(_ => {
      this.toastrService.success("Hra aktualizována.")
    }, err => {
      console.log(err);
      this.toastrService.error("Aktualizace se nezdařila.");
    });
  }

  updateGame(form: FormGroup): void {
    // FIXME: Use dirty flag to determine whether only /stock endpoint is not sufficient.
    let image = form.value['image'];
    delete form.value['category'];
    delete form.value['image'];
    if (image && image['file']) {
      this.imageUploadService.postFile(image['file']).subscribe(data => {
        form.patchValue({imageUrl: data.url});
        this.putGame(form.value);
      }, err => {
        if (err.status == HttpStatusCode.Conflict) {
          form.patchValue({imageUrl: err.error.url});
          this.putGame(form.value);
        } else {
          this.toastrService.error("Nepodařilo se nahrát obrázek na server.");
        }
      })
    } else {
      form.patchValue({imageUrl: this.game.imageUrl});
      this.putGame(form.value)
    }
  }

}
