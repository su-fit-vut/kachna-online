import { environment } from '../../environments/environment';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PlanCleaningComponent } from "./plan-cleaning/plan-cleaning.component";
import { CleaningManagerGuard } from "./cleaning-manager.guard";
import { CurrentCleaningsComponent } from "./current-cleanings/current-cleanings.component";
import { CleaningsArchiveComponent } from "./cleanings-archive/cleanings-archive.component";
import { EditCleaningsComponent } from './edit-cleanings/edit-cleanings.component';
import { CleaningDetailComponent } from './cleaning-detail/cleaning-detail.component';
// import { CleaningArchiveComponent } from "./cleaning-archive/cleaning-archive.component";

const routes: Routes = [
  { path: 'cleanings',
    children: [
      {
        path: '',
        children: [
          {
            path: 'current',
            component: CurrentCleaningsComponent,
            data: {
              title: `${environment.siteName} | Aktuální úklidy`,
              description: 'Přehled probíhajících úklidů',
            }
          },
          {
            path: 'plan',
            component: PlanCleaningComponent,
            canActivate: [CleaningManagerGuard],
            data: {
              title: `${environment.siteName} | Plánování úklidu`,
              description: 'Plánování nového úklidu',
            }
          },
          { 
            path: 'archive',
            component: CleaningsArchiveComponent,
            data: {
              title: `${environment.siteName} | Archiv úklidů`,
              description: 'Přehled proběhlých úklidů',
            }
          },
          {
            path: ':cleaningId',
            pathMatch: 'full',
            component: CleaningDetailComponent,
            data: {
              title: `${environment.siteName} | Detail úklidu`,
              description: 'Detailní popis úklidu',
            },
            children: [
              {
                path: '',
                component: CleaningDetailComponent,
                children: [
                ]
              }
            ]
          },
          {
            path: ':cleaningId/edit',
            pathMatch: 'full',
            component: EditCleaningsComponent,
            canActivate: [CleaningManagerGuard],
            data: {
              title: `${environment.siteName} | Úprava úklidu`,
              description: 'Úprava existujícího úklidu',
            }
          },
        ]
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CleaningRoutingModule { }