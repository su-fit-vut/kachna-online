// users-routing.module.ts
// Author: David Chocholatý

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './login/login.component';
import { UserProfileComponent } from "./user-profile/user-profile.component";
import { UserLoggedInGuard } from "./user-logged-in.guard";
import { UsersListComponent } from "./users-list/users-list.component";
import { AdminGuard } from "./admin.guard";
import { UserDetailComponent } from "./user-detail/user-detail.component";
import { CurrentEventsComponent } from "../events/current-events/current-events.component";
import { environment } from "../../environments/environment";
import { ManageUserRolesComponent } from "./user-detail/manage-user-roles/manage-user-roles.component";
import { NotificationSettingsComponent } from "./notification-settings/notification-settings.component";
import { RegistrationComponent } from "./registration/registration.component";

const routes: Routes = [
  /* Route order
The order of routes is important because the Router uses a first-match wins strategy when matching routes, so more specific routes should be placed above less specific routes. List routes with a static path first, followed by an empty path route, which matches the default route. The wildcard route comes last because it matches every URL and the Router selects it only if no other routes match first.
*/
  { path: 'login', component: LoginComponent },
  {
    path: 'notifications',
    component: NotificationSettingsComponent,
    data: {
      title: `${environment.siteName} | Nastavení notifikací`,
      description: 'Nastavení push notifikací'
    }
  },
  {
    path: 'user-profile',
    component: UserProfileComponent,
    canActivate: [UserLoggedInGuard],
  },
  {
    path: 'users',
    component: UsersListComponent,
    canActivate: [AdminGuard],
  },
  {
    path: 'registration',
    component: RegistrationComponent,
  },
  {
    path: 'users/:userId',
    pathMatch: 'full',
    component: UserDetailComponent,
    canActivate: [AdminGuard],
    data: {
      title: `${environment.siteName} | Detail uživatele`,
      description: 'Detail uživatele',
    }
  },
  {
    path: 'users/:userId/roles',
    pathMatch: 'full',
    component: ManageUserRolesComponent,
    canActivate: [AdminGuard],
    data: {
      title: `${environment.siteName} | Správá rolí uživatele`,
      description: 'Správa rolí uživatele',
    }
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersRoutingModule { }
