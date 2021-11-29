import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StatesRoutingModule } from './states-routing.module';
import { PlanStateComponent } from './plan-state/plan-state.component';
import { ReactiveFormsModule } from "@angular/forms";
import { NgbDateParserFormatter, NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { CustomDateParserFormatter } from "./local-date-parser-formatter";


@NgModule({
  declarations: [
    PlanStateComponent
  ],
  imports: [
    CommonModule,
    StatesRoutingModule,
    ReactiveFormsModule,
    NgbModule
  ],
  providers: [
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter}
  ]
})
export class StatesModule {
}

