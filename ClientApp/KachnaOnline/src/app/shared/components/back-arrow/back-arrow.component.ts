// back-arrow.component.ts
// Author: František Nečas, Ondřej Ondryáš

import { Component, Input, OnInit } from '@angular/core';
import { Location } from '@angular/common';

@Component({
  selector: 'app-back-arrow',
  templateUrl: './back-arrow.component.html',
  styleUrls: ['./back-arrow.component.css']
})
export class BackArrowComponent implements OnInit {
  @Input() targetRoute: string;

  @Input() headingSize: number = 2;
  @Input() heading: string;

  constructor(private location: Location) { }

  ngOnInit(): void {
  }

  back(): void {
    this.location.back();
  }
}
