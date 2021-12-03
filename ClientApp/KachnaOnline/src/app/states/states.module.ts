import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StatesRoutingModule } from './states-routing.module';
import { PlanStateComponent } from './plan-state/plan-state.component';
import { ReactiveFormsModule } from "@angular/forms";
import { NgbDateNativeAdapter, NgbDateParserFormatter, NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { CustomDateParserFormatter } from "./local-date-parser-formatter";
import { StatesListComponent } from './states-list/states-list.component';
import { ComponentsModule } from "../shared/components/components.module";
import { PipesModule } from "../shared/pipes/pipes.module";


@NgModule({
  declarations: [
    PlanStateComponent,
    StatesListComponent,
  ],
  imports: [
    CommonModule,
    StatesRoutingModule,
    ReactiveFormsModule,
    NgbModule,
    ComponentsModule,
    PipesModule
  ],
  providers: [
    {provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter},
    {provide: NgbDateNativeAdapter, useClass: NgbDateNativeAdapter}
  ]
})
export class StatesModule {
}

