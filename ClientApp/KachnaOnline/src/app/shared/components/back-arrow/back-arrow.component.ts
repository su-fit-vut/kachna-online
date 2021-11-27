// back-arrow.component.ts
// Author: František Nečas

import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-back-arrow',
  templateUrl: './back-arrow.component.html',
  styleUrls: ['./back-arrow.component.css']
})
export class BackArrowComponent implements OnInit {
  @Input() targetRoute: string

  constructor() { }

  ngOnInit(): void {
  }

}
