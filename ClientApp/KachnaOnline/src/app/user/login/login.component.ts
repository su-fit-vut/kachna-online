import { UserService } from './../../shared/services/user.service';
import { ToastrService } from 'ngx-toastr';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private toastr: ToastrService,
    private userService: UserService,
  ) { }

  ngOnInit(): void {
    let sessionId = "";
    this.route.queryParams
    .subscribe(params => {
      sessionId = params['session'];
    });

    let params = new HttpParams().set('session', sessionId);

    this.http.get(`${environment.baseApiUrl}/auth/accessTokenFromSession`, { params: params, responseType: 'text'}).subscribe(
      res => {
        this.userService.authenticationToken = res;
      },
      err => {
        console.log(err);
        this.toastr.error("Načtení přístupových údajů selhalo.", "Autentizace");
        return;
      }
    );
  }
}
