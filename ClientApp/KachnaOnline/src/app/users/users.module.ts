// users.module.ts
// Author: David Chocholatý

import { environment } from '../../environments/environment';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsersRoutingModule } from './users-routing.module';
import { UsersListComponent } from './users-list/users-list.component';
import { UserProfileComponent } from "./user-profile/user-profile.component";
import { FormsModule, NgForm } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { LoginComponent } from "./login/login.component";
import { UserDetailComponent } from './user-detail/user-detail.component';
import { ManageUserRolesComponent } from './user-detail/manage-user-roles/manage-user-roles.component';
import { ComponentsModule } from "../shared/components/components.module";
import { UserRoleComponent } from './user-detail/manage-user-roles/user-role/user-role.component';
import { NotificationSettingsComponent } from './notification-settings/notification-settings.component';

@NgModule({
  declarations: [
    UsersListComponent,
    UserProfileComponent,
    LoginComponent,
    UserDetailComponent,
    ManageUserRolesComponent,
    UserRoleComponent,
    NotificationSettingsComponent,
  ],
  imports: [
    CommonModule,
    UsersRoutingModule,
    FormsModule,
    NgbModule,
    ComponentsModule,
  ]
})
export class UsersModule { }
