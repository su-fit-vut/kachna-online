import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/shared/services/user.service';

@Component({
  selector: 'app-logged-out-content',
  templateUrl: './logged-out-content.component.html',
  styleUrls: ['./logged-out-content.component.css']
})
export class LoggedOutContentComponent implements OnInit {

  constructor(
    public userService: UserService,
  ) { }

  ngOnInit(): void {
  }

  clickLogInButton() {
    this.userService.logIn();
  }

  clickRegisterButton() {

  }

}
