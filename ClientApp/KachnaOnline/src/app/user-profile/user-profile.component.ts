import { Component, OnInit } from '@angular/core';
import {UserService} from "../shared/services/user.service";
import {NgForm} from "@angular/forms";

@Component({
  selector: 'app-user',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {
  public isCardInfoCollapsed: boolean = false;

  constructor(
    public userService: UserService,
  ) { }

  ngOnInit(): void {
    this.userService.getInformationAboutUser();
  }

  onSaveChanges(form: NgForm) {
    // TODO: Save changes to user nickname, card and gamification approval.
  }
}
