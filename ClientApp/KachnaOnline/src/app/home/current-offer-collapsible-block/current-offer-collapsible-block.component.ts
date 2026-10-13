import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { ClubState } from "../../models/states/club-state.model";
import { ClubStateTypes } from "../../models/states/club-state-types.model";

@Component({
  selector: 'app-current-offer-collapsible-block',
  templateUrl: './current-offer-collapsible-block.component.html',
  styleUrls: ['./current-offer-collapsible-block.component.css', '../home.component.css']
})
export class CurrentOfferCollapsibleBlockComponent {
  @Input() state: ClubState
  @Input()
  set unroll(operation: boolean) {
    if (operation) {
      this.currentOfferCollapsed = false;
    }
  }
  currentOfferCollapsed: boolean = true;

  @Output() offerLoaded = new EventEmitter<void>();

  ST = ClubStateTypes;

  onOfferLoaded() {
    this.offerLoaded.emit();
  }
}
