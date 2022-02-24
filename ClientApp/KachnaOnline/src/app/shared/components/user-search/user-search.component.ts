import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { UserDetail } from "../../../models/users/user.model";
import { AuthenticationService } from "../../services/authentication.service";
import { Observable, of } from "rxjs";
import { catchError, debounceTime, distinctUntilChanged, map, switchMap, tap } from "rxjs/operators";

@Component({
  selector: 'app-user-search',
  templateUrl: './user-search.component.html',
  styleUrls: ['./user-search.component.css']
})
export class UserSearchComponent implements OnInit {
  @Input() label: string
  @Output() userSelected: EventEmitter<UserDetail> = new EventEmitter();
  model: UserDetail[]
  searching: boolean;
  searchFailed: boolean;
  noUsersFound: boolean;

  constructor(private authService: AuthenticationService) {
  }

  ngOnInit(): void {
  }

  itemSelected(event: any) {
    this.userSelected.emit(event.item);
  }

  formatter = (x: UserDetail) => {
    let result = `${x.name} (`
    if (x.nickname) {
      result += `${x.nickname}, `
    }
    result += `${x.email})`;
    return result;
  };

  search = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      tap(() => {
        this.searching = true;
        this.noUsersFound = false;
      }),
      switchMap(term =>
        term.length < 3 ? [] :
          this.authService.getFilteredUsers(term).pipe(
            tap(() => this.searchFailed = false),
            map(x => x.slice(0, 10)),
            catchError(e => {
              this.searchFailed = true;
              return of([]);
            }),
            tap(res => this.noUsersFound = res.length == 0)
          )),
      tap(() => this.searching = false)
    )
}

