// users.module.ts
// Author: David Chocholatý

import { environment } from '../../environments/environment';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsersRoutingModule } from './users-routing.module';
import { UsersListComponent } from './users-list/users-list.component';
import {UserProfileComponent} from "./user-profile/user-profile.component";
import {FormsModule, NgForm} from "@angular/forms";
import {NgbModule} from "@ng-bootstrap/ng-bootstrap";
import {LoginComponent} from "./login/login.component";

@NgModule({
  declarations: [
    UsersListComponent,
    UserProfileComponent,
    LoginComponent,
  ],
  imports: [
    CommonModule,
    UsersRoutingModule,
    FormsModule,
    NgbModule,
  ]
})
export class UsersModule { }
