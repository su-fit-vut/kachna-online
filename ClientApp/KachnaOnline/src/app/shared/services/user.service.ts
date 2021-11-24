import { HttpClient } from '@angular/common/http';
import { AuthenticationService } from './authentication.service';
import { Injectable } from '@angular/core';
import {User} from "../../models/user.model";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(
    private authenticationService: AuthenticationService,
    private http: HttpClient,
  ) { }

  userDetail: User = new User();

  public authenticationToken: string;

  logIn() {
    this.authenticationService.getSessionIdFromKisEduId();
  }

  logOut() {
    this.authenticationService.logOut();
  }

  getInformationAboutUser() {
    this.http.get<any>('https://su-int.fit.vutbr.cz/kis/api/users/me').subscribe( // TODO: Change return value into a model.
      res => {
        console.log(res);
      }
    );

    this.assignDataFromLocalTokenContent();
  }

  getUserName() {
    return this.userDetail.name;
  }

  isLoggedIn() {
    return this.authenticationService.isLoggedIn();
  }

  getPrestigeAmount() {
    return 42;
  }

  getUserEmail() {
    return this.userDetail.email;
  }

  private assignDataFromLocalTokenContent() {
    this.userDetail.name = this.authenticationService.localTokenContent.given_name;
    this.userDetail.email = this.authenticationService.localTokenContent.email;
  }
}
