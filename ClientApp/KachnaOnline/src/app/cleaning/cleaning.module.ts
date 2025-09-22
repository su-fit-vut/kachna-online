import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap'
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { PlanCleaningComponent } from './plan-cleaning/plan-cleaning.component';
import { ComponentsModule } from '../shared/components/components.module';
import { CleaningRoutingModule } from './cleaning-routing.module';
import { CleaningFormComponent } from './cleaning-form/cleaning-form.component';
import { CurrentCleaningsComponent } from './planned-cleanings/planned-cleanings.component';
import { CleaningsArchiveComponent } from './cleanings-archive/cleanings-archive.component';
import { EditCleaningsComponent } from './edit-cleanings/edit-cleanings.component';
import { CleaningDetailComponent } from './cleaning-detail/cleaning-detail.component';
import { PipesModule } from '../shared/pipes/pipes.module';

@NgModule({
  declarations: [
    PlanCleaningComponent,
    CleaningFormComponent,
    CurrentCleaningsComponent,
    CleaningsArchiveComponent,
    EditCleaningsComponent,
    CleaningDetailComponent,
  ],
  imports: [
    CommonModule,
    CleaningRoutingModule,
    NgbModule,
    FormsModule,
    ComponentsModule,
    PipesModule,
    ReactiveFormsModule,
  ]
})
export class CleaningModule { }