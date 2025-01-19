import { Component, Input } from '@angular/core';
import { ClubState } from "../../models/states/club-state.model";
import { ClubStateTypes } from "../../models/states/club-state-types.model";

@Component({
  selector: 'app-current-offer-collapsible-block',
  templateUrl: './current-offer-collapsible-block.component.html',
  styleUrls: ['./current-offer-collapsible-block.component.css', '../home.component.css']
})
export class CurrentOfferCollapsibleBlockComponent {
  currentOfferCollapsed: boolean = true;
  @Input() state: ClubState;

  ST = ClubStateTypes;
}
