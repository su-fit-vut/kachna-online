import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import {CommonModule} from "@angular/common";
import {PipesModule} from "../../shared/pipes/pipes.module";
import { ClubInfoService } from "../../shared/services/club-info.service";
import { ClubState } from "../../models/states/club-state.model";

@Component({
  selector: 'app-tearoom-details',
  templateUrl: './tearoom-details.component.html',
  styleUrls: ['./tearoom-details.component.css', '../home.component.css']
})
export class TearoomDetailsComponent {
  constructor(public service: ClubInfoService) { }

  @Input() state: ClubState;
}
