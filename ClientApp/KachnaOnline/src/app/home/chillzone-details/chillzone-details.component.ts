// chillzone-details.component.ts
// Author: Ondřej Ondryáš

import { Component, Input, OnInit } from '@angular/core';
import { ClubState } from "../../models/states/club-state.model";

@Component({
  selector: 'app-chillzone-details',
  templateUrl: './chillzone-details.component.html',
  styleUrls: ['./chillzone-details.component.css', '../home.component.css']
})
export class ChillzoneDetailsComponent implements OnInit {

  constructor() {
  }

  @Input() state: ClubState;

  ngOnInit(): void {
  }

}
