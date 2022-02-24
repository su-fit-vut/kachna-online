import { Component, OnInit } from '@angular/core';
import { UserDetail } from "../../../models/users/user.model";
import { AuthenticationService } from "../../../shared/services/authentication.service";
import { ActivatedRoute } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { RoleTypes } from "../../../models/users/auth/role-types.model";
import { forkJoin, Observable } from "rxjs";

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
      .then((userDetail: UserDetail) => {
        this.userDetail = userDetail;
      }).catch((err) => {
      console.log(err);
      this.toastr.error("Nepodařilo se získat data o uživateli.", "Správa uživatelů");
    });
  }

  onRemoveUserRole(userRole: string) {
    if (userRole == "Admin" && this.userDetail.id == this.authenticationService.user.id) {
      this.toastr.error("Nemůžete si odebrat vlastní administrátorskou roli.", "Správa uživatelů");
      return;
    }

    this.authenticationService.removeUserRoleRequest(this.userDetail.id, userRole).toPromise()
      .then(() => {
        this.toastr.success("Nastaveno vynucené zakázání role.", "Správa uživatelů");

        this.route.paramMap.subscribe(params => {
          let userId = Number(params.get('userId'));
          this.getUserDetailData(userId);
        });
      }).catch((err) => {
      console.log(err);
      this.toastr.error("Nepodařilo se nastavit zakázání role.", "Správa uživatelů");
    });
  }

  onRemoveAllUserRoles() {
    let requests: Observable<any>[] = [];

    for (let userRole of this.userDetail.activeRoles) {
      if (userRole == "Admin" && this.userDetail.id == this.authenticationService.user.id) {
        continue;
      }

      let req = this.authenticationService.removeUserRoleRequest(this.userDetail.id, userRole);
      requests.push(req);

      req.toPromise().catch((err) => {
        console.log(err);
        this.toastr.error("Nepodařilo se nastavit zakázání role.", "Správa uživatelů");
      });
    }

    forkJoin(requests).subscribe(_ => {
      this.toastr.success("Nastaveno vynucené zakázání pro všechny role.", "Správa uživatelů");

      this.route.paramMap.subscribe(params => {
        let userId = Number(params.get('userId'));
        this.getUserDetailData(userId);
      });
    });
  }

  onUserRoleEnabled($event: string) {
    this.authenticationService.addUserRoleRequest(this.userDetail.id, $event).toPromise()
      .then(() => {
        this.toastr.success("Role byla vynuceně přidána.", "Správa uživatelů");

        this.route.paramMap.subscribe(params => {
          let userId = Number(params.get('userId'));
          this.getUserDetailData(userId);
        });
      }).catch((err) => {
      console.log(err);
      this.toastr.error("Nepodařilo se vynuceně přidat roli.", "Správa uživatelů");
    });

  }

  onUserRoleDisabled($event: string) {
    this.onRemoveUserRole($event);
  }

  onResetRolesToKisRoles(userRole: string) {
    this.authenticationService.resetUserRoleRequest(this.userDetail.id, userRole).toPromise()
      .then(() => {
        this.toastr.success("Role úspěšně resetována, při příštím přihlášení uživatele bude namapována podle KIS.", "Správa uživatelů");
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
