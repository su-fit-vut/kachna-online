import { UserService } from '../../shared/services/user.service';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-account-popup',
  templateUrl: './account-popup.component.html',
  styleUrls: ['./account-popup.component.css']
})
export class AccountPopupComponent implements OnInit {

  constructor(public userService: UserService) { }

  UserAccountPopoverTextContent: string = "";
  LogInOutButtonText = "Přihlásit se";

  ngOnInit(): void {
    this.setUserAccountPopoverContent();
    this.userService.onAccountPopupInitialized(this.setUserAccountPopoverContent.bind(this));
  }

  setUserAccountPopoverContent() {
    this.setLogInOutButtonText();
    this.setUserAccountPopoverTextContent();

  }

  setUserAccountPopoverTextContent() {
    if (this.userService.LoggedIn == true) {
      this.UserAccountPopoverTextContent = this.getUserName();
    } else {
      this.UserAccountPopoverTextContent = "Nejste přihlášen/a.";
    }
  }

  setLogInOutButtonText() {
    if (this.userService.LoggedIn == true) {
      this.LogInOutButtonText = "Odhlásit se";
    } else {
      this.LogInOutButtonText = "Přihlásit se";
    }
  }

  clickLogInOutButton() {
    // TODO: Do the actual Log in/out.
    this.userService.LoggedIn = !this.userService.LoggedIn;
  }

  getUserName() {
    return "David Chocholatý";
  }
}
