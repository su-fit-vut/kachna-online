// push-notification-service.ts
// Author: František Nečas

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import { PushConfiguration } from "../../models/users/push-configuration.model";

enum ApiPaths {
  PublicKey = "/publicKey",
  Subscriptions = "/subscriptions"
}

@Injectable({
  providedIn: 'root'
})
export class PushNotificationsService {
  readonly PushUrl = environment.baseApiUrl + '/push';

  constructor(private http: HttpClient) { }

  getPublicKey(): Observable<any> {
    const headers = new HttpHeaders().set('Content-Type', 'text/plain; charset=utf-8');
    const requestOptions: Object = {
      headers: headers,
      responseType: 'text'
    }
    return this.http.get<any>(`${this.PushUrl}${ApiPaths.PublicKey}` , requestOptions);
  }

  subscribe(subscription: PushSubscription, configuration: PushConfiguration): Observable<any> {
    return this.http.put(`${this.PushUrl}${ApiPaths.Subscriptions}`,
      {subscription: subscription, configuration: configuration});
  }

  unsubscribe(endpoint: string): Observable<any> {
    let encoded = encodeURIComponent(endpoint);
    return this.http.delete(`${this.PushUrl}${ApiPaths.Subscriptions}/${encoded}`);
  }

  getConfiguration(endpoint: string): Observable<any> {
    let encoded = encodeURIComponent(endpoint);
    return this.http.get(`${this.PushUrl}${ApiPaths.Subscriptions}/${encoded}`);
  }
}
