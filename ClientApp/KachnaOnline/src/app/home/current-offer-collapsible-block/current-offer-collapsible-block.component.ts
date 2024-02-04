import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-current-offer-collapsible-block',
  templateUrl: './current-offer-collapsible-block.component.html',
  styleUrls: ['./current-offer-collapsible-block.component.css', '../home.component.css']
})
export class CurrentOfferCollapsibleBlockComponent {
  currentOfferCollapsed: boolean = true;
  @Input() tearoomMode: boolean = false;
}
