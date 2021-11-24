import { HttpClient } from '@angular/common/http';
import { AuthenticationService } from './authentication.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private authenticationService: AuthenticationService,
    private http: HttpClient,
  ) { }

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
  }

  getUserName() {
    return 'David Chocholatý';
  }

  isLoggedIn() {
    return this.authenticationService.isLoggedIn();
  }

  getPrestigeAmount() {
    return 42;
  }
}
