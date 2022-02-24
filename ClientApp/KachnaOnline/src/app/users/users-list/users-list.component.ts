import { Component, OnInit } from '@angular/core';
import { EventsService } from "../../shared/services/events.service";
import { ClubState } from "../../models/states/club-state.model";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { AuthenticationService } from "../../shared/services/authentication.service";
import { User, UserDetail } from "../../models/users/user.model";

@Component({
  selector: 'app-user-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.css']
})
export class UsersListComponent implements OnInit {
  constructor(
    public authenticationService: AuthenticationService,
    public router: Router,
    private toastr: ToastrService,
    private route: ActivatedRoute,
  ) {
  }

  ngOnInit(): void {
    this.authenticationService.refreshUsersList();
  }

  openUserDetail(user: UserDetail) {
    this.router.navigate([`/users/${user.id}`]).then();
  }

  onModifyUserRoles(user: UserDetail) {
    this.router.navigate([`/users/${user.id}/roles`]).then();
  }
}
