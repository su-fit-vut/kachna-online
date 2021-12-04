// loading-spinner.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { LoaderService } from "../../services/loader.service";

@Component({
  selector: 'app-loading-spinner',
  templateUrl: './loading-spinner.component.html',
  styleUrls: ['./loading-spinner.component.css']
})
export class LoadingSpinnerComponent implements OnInit {

  constructor(public loaderService: LoaderService) { }

  ngOnInit(): void {
  }

}
