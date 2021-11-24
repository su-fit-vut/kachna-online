import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './login/login.component';
import {UserProfileComponent } from "./user-profile/user-profile.component";
import {UserLoggedInGuard } from "./user-profile/user-logged-in.guard";
import {UsersListComponent} from "./users-list/users-list.component";
import {AdminGuard} from "./admin.guard";

const routes: Routes = [
  /* Route order
The order of routes is important because the Router uses a first-match wins strategy when matching routes, so more specific routes should be placed above less specific routes. List routes with a static path first, followed by an empty path route, which matches the default route. The wildcard route comes last because it matches every URL and the Router selects it only if no other routes match first.
*/
  { path: 'login', component: LoginComponent },
  {
    path: 'user_profile',
    component: UserProfileComponent,
    canActivate: [UserLoggedInGuard],
  },
  {
    path: 'users',
    component: UsersListComponent,
    canActivate: [AdminGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersRoutingModule { }
