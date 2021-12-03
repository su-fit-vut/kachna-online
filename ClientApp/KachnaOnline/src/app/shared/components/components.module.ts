import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BackArrowComponent } from "./back-arrow/back-arrow.component";
import { RouterModule } from "@angular/router";
import { ToggleableButtonComponent } from "./togglable-button/toggleable-button.component";
import { NumberSelectionComponent } from "./number-selection/number-selection.component";
import { ReactiveFormsModule } from "@angular/forms";
import { UserSearchComponent } from './user-search/user-search.component';
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { FormsModule } from "@angular/forms";


@NgModule({
  declarations: [
    BackArrowComponent,
    ToggleableButtonComponent,
    NumberSelectionComponent,
    UserSearchComponent,
  ],
  imports: [
    RouterModule,
    CommonModule,
    ReactiveFormsModule,
    NgbModule,
    FormsModule
  ],
  exports: [
    BackArrowComponent,
    ToggleableButtonComponent,
    NumberSelectionComponent,
    UserSearchComponent,
  ]
})
export class ComponentsModule {
}
