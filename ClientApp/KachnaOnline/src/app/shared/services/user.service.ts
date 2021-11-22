import { HttpClient } from '@angular/common/http';
import { AuthenticationService } from './authentication.service';
import { Injectable } from '@angular/core';

import { AccountPopupComponent } from '../../navigation-bar/account-popup/account-popup.component';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private authentizationService: AuthenticationService,
    private http: HttpClient,
  ) { }

  private _loggedIn: boolean = false;

  public authenticationToken: string;

  private setUserAccountPopoverContent: () => void;

  onAccountPopupInitialized(fn: () => void) {
    this.setUserAccountPopoverContent = fn;
  }

  get loggedIn(): boolean {
    return this._loggedIn;
  }

  set loggedIn(value: boolean) {
      if (value !== this._loggedIn) {
          this._loggedIn= value;
          this.onLoggedInChanged();
      }
  }

  onLoggedInChanged() {
    this.setUserAccountPopoverContent();
  }

  logIn() {
    this.authentizationService.getSessionIdFromKisEduId();
  }

  logOut() {
    this.authentizationService.logOut();
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
    return this.authentizationService.isLoggedIn();
  }

  getPrestigeAmount() {
    return 42;
  }
}
