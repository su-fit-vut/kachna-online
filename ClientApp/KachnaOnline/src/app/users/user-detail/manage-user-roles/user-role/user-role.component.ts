import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { RoleTypes } from "../../../../models/users/auth/role-types.model";

@Component({
  selector: 'app-user-role',
  templateUrl: './user-role.component.html',
  styleUrls: ['./user-role.component.css']
})
export class UserRoleComponent implements OnInit {
  @Input() userRole: RoleTypes;
  @Input() startingValue: boolean = false;
  @Output() userRoleEnabled: EventEmitter<string> = new EventEmitter();
  @Output() userRoleDisabled: EventEmitter<string> = new EventEmitter();

  constructor() {
  }

  ngOnInit(): void {
  }

  onCheckboxChange(e: any): void {
    if (e.target.checked) {
      this.userRoleEnabled.emit(this.userRole);
    } else {
      this.userRoleDisabled.emit(this.userRole);
    }
  }
}
