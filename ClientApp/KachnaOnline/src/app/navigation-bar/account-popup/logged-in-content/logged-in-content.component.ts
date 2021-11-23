import { UserService } from '../../../shared/services/user.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-logged-in-content',
  templateUrl: './logged-in-content.component.html',
  styleUrls: ['./logged-in-content.component.css']
})
export class LoggedInContentComponent implements OnInit {

  constructor(
    public userService: UserService,
  ) { }

  ngOnInit(): void {
  }

  clickMyReservationsButton() {

  }

  clickChangeCard() {

  }

  clickLogOutButton() {
    this.userService.logOut();
  }

}
