// Based on code from:
// https://gist.github.com/martinobordin/39bb1fe3400a29c1078dec00ff76bba9 (Martino Bordin)

import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from "rxjs/operators";

export class JsonDateInterceptor implements HttpInterceptor {
  private isoDateFormat = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d*)?Z?$/;

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(tap((val: HttpEvent<any>) => {
      if (val instanceof HttpResponse) {
        const body = val.body;
        this.convertDates(body);
      }
    }));
  }

  isIsoDateString(value: any): boolean {
    return (typeof value !== 'string') ? false : this.isoDateFormat.test(value)
  }

  convertDates(body: any) {
    if (body === null || body === undefined || typeof body !== 'object') {
      return;
    }

    for (const key of Object.keys(body)) {
      const value = body[key];
      if (this.isIsoDateString(value)) {
        body[key] = new Date(value);
      } else if (typeof value === 'object') {
        this.convertDates(value);
      }
    }
  }
}
