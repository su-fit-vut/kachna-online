// events-manager.guard.ts
// Author: David Chocholatý

import { Injectable} from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthenticationService } from "../shared/services/authentication.service";
import { RoleTypes } from "../models/users/auth/role-types.model";

@Injectable({
  providedIn: 'root'
})
export class EventsManagerGuard implements CanActivate {
  constructor(
    private authenticationService: AuthenticationService,
    private router: Router,
  ) {
  }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (this.authenticationService.hasRole(RoleTypes.EventsManager) || this.authenticationService.hasRole(RoleTypes.Admin)) {
      return true;
    } else {
      return this.router.parseUrl('/forbidden');
    }
  }

}
