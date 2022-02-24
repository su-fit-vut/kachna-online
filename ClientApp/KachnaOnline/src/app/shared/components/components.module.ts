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
import { MonthSelectionComponent } from './month-selection/month-selection.component';
import { PipesModule } from "../pipes/pipes.module";
import { DeletionConfirmationModalComponent } from './deletion-confirmation-modal/deletion-confirmation-modal.component';
import { YearSelectionComponent } from "./year-selection/year-selection.component";


@NgModule({
  declarations: [
    BackArrowComponent,
    ToggleableButtonComponent,
    NumberSelectionComponent,
    UserSearchComponent,
    MonthSelectionComponent,
    YearSelectionComponent,
    DeletionConfirmationModalComponent,
  ],
  imports: [
    RouterModule,
    CommonModule,
    ReactiveFormsModule,
    NgbModule,
    FormsModule,
    PipesModule
  ],
    exports: [
        BackArrowComponent,
        ToggleableButtonComponent,
        NumberSelectionComponent,
        UserSearchComponent,
        YearSelectionComponent,
        MonthSelectionComponent,
    ]
})
export class ComponentsModule {
}
