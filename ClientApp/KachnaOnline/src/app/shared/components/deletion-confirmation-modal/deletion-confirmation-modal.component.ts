// deletion-confirmation-modal.component.ts
// Author: David Chocholatý

import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: 'app-deletion-confirmation-modal',
  templateUrl: './deletion-confirmation-modal.component.html',
  styleUrls: ['./deletion-confirmation-modal.component.css']
})
export class DeletionConfirmationModalComponent implements OnInit {
  constructor(
    public modal: NgbActiveModal,
  ) {}

  @Input() type: DeletionType;
  @Input() name: string;

  typeText: string = "";
  typeContent: string = "";
  typeTitle: string = "";

  ngOnInit(): void {
    switch (this.type) {
      case DeletionType.Event:
        this.typeTitle = "Smazání akce";
        this.typeText = "Opravdu chcete zrušit akci";
        this.typeContent = "Akce bude smazána a všechny na ní navázané stavy uvolněny.";
        break;
      case DeletionType.BoardGame:
        break;
      case DeletionType.LinkedState:
        this.typeTitle = "Zrušení napojeného stavu";
        this.typeText = "Opravdu chcete zrušit stav s ID";
        this.typeContent = "Stav bude odstraněn a jeho navázání na akci zrušeno.";
        break;
      case DeletionType.LinkedStates:
        this.typeTitle = "Zrušení všech napojených stavů";
        this.typeText = "Opravdu chcete zrušit všechny napojené stavy na akci";
        this.typeContent = "Stavy budou odstraněny a jejich navázání na akci zrušeno.";
        break;
    }
  }
}

export enum DeletionType {
  Event,
  ClubState,
  LinkedState,
  LinkedStates,
BoardGame,
}
