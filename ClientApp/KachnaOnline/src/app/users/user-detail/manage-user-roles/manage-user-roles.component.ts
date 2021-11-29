// manager-user-roles.component.ts
// Author: David Chocholatý

import { Component, OnInit } from '@angular/core';
import { UserDetail } from "../../../models/users/user.model";
import { AuthenticationService } from "../../../shared/services/authentication.service";
import { ActivatedRoute } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { RoleTypes } from "../../../models/users/auth/role-types.model";

@Component({
  selector: 'app-manage-user-roles',
  templateUrl: './manage-user-roles.component.html',
  styleUrls: ['./manage-user-roles.component.css']
})
export class ManageUserRolesComponent implements OnInit {

  userDetail: UserDetail = new UserDetail();
  roleTypes: RoleTypes[] = [RoleTypes.BoardGamesManager, RoleTypes.Admin, RoleTypes.EventsManager, RoleTypes.StatesManager]

  constructor(
    public authenticationService: AuthenticationService,
    public route: ActivatedRoute,
    public toastr: ToastrService,
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      let userId = Number(params.get('userId'));
      this.getUserDetailData(userId);
    });
  }

  getUserDetailData(userId: number) {
    this.authenticationService.getUserDetailRequest(userId).toPromise()
      .then( (userDetail: UserDetail) => {
        this.userDetail = userDetail;
      }).catch((err) => {
      console.log(err);
      this.toastr.error("Nepodařilo se získat data o uživateli.", "Správa uživatelů");
    });
  }

  onRemoveUserRole(userRole: string) {
    this.authenticationService.removeUserRoleRequest(this.userDetail.id, userRole).toPromise()
      .then( () => {
        this.toastr.success("Role úspěšně odebrána.", "Správa uživatelů");

        this.route.paramMap.subscribe(params => {
          let userId = Number(params.get('userId'));
          this.getUserDetailData(userId);
        });
      }).catch((err) => {
      console.log(err);
      this.toastr.error("Nepodařilo se odebrat roli.", "Správa uživatelů");
    });
  }

  onRemoveAllUserRoles() {
    for (let userRole of this.userDetail.activeRoles) {
      this.authenticationService.removeUserRoleRequest(this.userDetail.id, userRole).toPromise()
        .then( () => {
        }).catch((err) => {
        console.log(err);
        this.toastr.error("Nepodařilo se odebrat roli.", "Správa uživatelů");
      });
    }

    this.toastr.success("Všechny Role úspěšně odebrány.", "Správa uživatelů");
  }

  onUserRoleEnabled($event: string) {
    this.authenticationService.addUserRoleRequest(this.userDetail.id, $event).toPromise()
      .then( () => {
        this.toastr.success("Role úspěšně přidána.", "Správa uživatelů");

        this.route.paramMap.subscribe(params => {
          let userId = Number(params.get('userId'));
          this.getUserDetailData(userId);
        });
      }).catch((err) => {
      console.log(err);
      this.toastr.error("Nepodařilo se přidat roli.", "Správa uživatelů");
    });

  }

  onUserRoleDisabled($event: string) {
    this.onRemoveUserRole($event);
  }

  onResetRolesToKisRoles(userRole: string) {
    this.authenticationService.resetUserRoleRequest(this.userDetail.id, userRole).toPromise()
      .then( () => {
        this.toastr.success("Role úspěšně resetová.", "Správa uživatelů");
        this.route.paramMap.subscribe(params => {
          let userId = Number(params.get('userId'));
          this.getUserDetailData(userId);
        });
      }).catch((err) => {
      console.log(err);
      this.toastr.error("Nepodařilo se resetovat roli.", "Správa uživatelů");
    });
  }
}
