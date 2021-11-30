// loading.interceptor.ts
// Author: František Nečas
// Inspired by: https://stackoverflow.com/questions/49385369/angular-show-spinner-for-every-http-request-with-very-less-code-changes

import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from "@angular/common/http";
import { LoaderService } from "../services/loader.service";
import { Observable } from "rxjs";
import { Injectable } from "@angular/core";

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  private pendingRequests: HttpRequest<any>[] = [];
  private handle: number | null = null;
  private loadingBackoff: number = 150;

  constructor(private loaderService: LoaderService) {
  }

  removeRequest(req: HttpRequest<any>): void {
    const index = this.pendingRequests.indexOf(req);
    this.pendingRequests.splice(index, 1);
    this.loaderService.loading.next(this.pendingRequests.length > 0);
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.pendingRequests.push(req);
    if (this.handle === null) {
      // On fact connection, there is no reason for a spinning wheel, show it only if the page is loading "long"
      this.handle = setTimeout(() => {
        if (this.pendingRequests.length > 0) {
          this.loaderService.loading.next(true);
        }
        this.handle = null;
      }, this.loadingBackoff);
    }
    return new Observable(observer => {
      const subscription = next.handle(req).subscribe(event => {
        if (event instanceof HttpResponse) {
          this.removeRequest(req);
          observer.next(event);
        }
      }, err => {
        this.removeRequest(req);
        observer.error(err);
      }, () => {
        this.removeRequest(req);
        observer.complete();
      });
      return () => {
        this.removeRequest(req);
        subscription.unsubscribe();
      }
    });
  }
}
