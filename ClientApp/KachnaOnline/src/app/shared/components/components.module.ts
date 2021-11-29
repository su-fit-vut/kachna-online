import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BackArrowComponent } from "./back-arrow/back-arrow.component";
import { RouterModule } from "@angular/router";
import { TogglableButtonComponent } from "./togglable-button/togglable-button.component";
import { NumberSelectionComponent } from "./number-selection/number-selection.component";
import { ReactiveFormsModule } from "@angular/forms";


@NgModule({
  declarations: [
    BackArrowComponent,
    TogglableButtonComponent,
    NumberSelectionComponent,
  ],
  imports: [
    RouterModule,
    CommonModule,
    ReactiveFormsModule,
  ],
  exports: [
    BackArrowComponent,
    TogglableButtonComponent,
    NumberSelectionComponent,
  ]
})
export class ComponentsModule { }
