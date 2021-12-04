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
      case DeletionType.State:
        break;
    }
  }
}

export enum DeletionType {
  Event,
  State,
  BoardGame,
}
