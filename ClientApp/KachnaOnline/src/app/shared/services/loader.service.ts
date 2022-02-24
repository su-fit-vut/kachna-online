import { Injectable } from '@angular/core';
import { BehaviorSubject } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class LoaderService {
  public loading = new BehaviorSubject<boolean>(false);

  constructor() { }
}
