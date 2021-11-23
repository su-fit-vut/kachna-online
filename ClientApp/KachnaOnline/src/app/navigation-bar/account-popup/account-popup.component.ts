import { UserService } from '../../shared/services/user.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-account-popup',
  templateUrl: './account-popup.component.html',
  styleUrls: ['./account-popup.component.css']
})
export class AccountPopupComponent implements OnInit {

  constructor(public userService: UserService) { }

  ngOnInit(): void {
  }

  getUserName() {
    return "David Chocholatý"; // FIXME: Implement.
  }
}
