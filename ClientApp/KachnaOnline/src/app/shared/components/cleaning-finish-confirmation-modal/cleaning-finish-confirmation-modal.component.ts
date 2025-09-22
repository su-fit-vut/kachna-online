import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: 'cleaning-finish-confirmation-modal',
  templateUrl: './cleaning-finish-confirmation-modal.component.html',
  styleUrls: ['./cleaning-finish-confirmation-modal.component.css']
})
export class CleaningFinishConfirmationModalComponent implements OnInit {
  constructor(
    public modal: NgbActiveModal,
  ) {}

  @Input() name: string;

  typeText: string = "";
  typeContent: string = "";
  typeTitle: string = "";

  ngOnInit(): void {
    this.typeTitle = "Dokončení úklidu";
    this.typeText = "Opravdu chcete označit úklid jako dokončen";
    this.typeContent = "Úklid bude dokončen.";
  }
}
