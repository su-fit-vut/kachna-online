import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BackArrowComponent } from "./back-arrow/back-arrow.component";
import { RouterModule } from "@angular/router";


@NgModule({
  declarations: [
    BackArrowComponent
  ],
  imports: [
    RouterModule,
    CommonModule,
  ],
  exports: [
    BackArrowComponent
  ]
})
export class ComponentsModule { }
