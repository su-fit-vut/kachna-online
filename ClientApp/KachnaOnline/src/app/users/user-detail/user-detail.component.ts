import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from "../../shared/services/authentication.service";
import { UserDetail } from "../../models/users/user.model";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.css']
})
export class UserDetailComponent implements OnInit {
  userDetail: UserDetail = new UserDetail();

  constructor(
    public authenticationService: AuthenticationService,
    public route: ActivatedRoute,
    public toastr: ToastrService,
    public router: Router,
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

  onModifyButtonClicked() {

  }

  onManageUserRolesClicked() {
    this.route.paramMap.subscribe(params => {
      let userId = Number(params.get('userId'));
      this.router.navigate([`/users/${userId}/roles`]).then();
    });
  }
}
