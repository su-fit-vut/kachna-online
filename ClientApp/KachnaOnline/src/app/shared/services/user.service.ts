import { AuthenticationService } from './authentication.service';
import { Injectable } from '@angular/core';

import { AccountPopupComponent } from '../../navigation-bar/account-popup/account-popup.component';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private authentizationService: AuthenticationService,
  ) { }

  private _LoggedIn: boolean = false;

  public authenticationToken: string;

  private setUserAccountPopoverContent: () => void;

  onAccountPopupInitialized(fn: () => void) {
    this.setUserAccountPopoverContent = fn;
  }

  get LoggedIn(): boolean {
    return this._LoggedIn;
  }

  set LoggedIn(value: boolean) {
      if (value !== this._LoggedIn) {
          this._LoggedIn= value;
          this.onLoggedInChanged();
      }
  }

  onLoggedInChanged() {
    this.setUserAccountPopoverContent();
  }

  onLogInButtonsClicked() {
    this.authentizationService.getSessionIdFromKisEduId();
  }
}
