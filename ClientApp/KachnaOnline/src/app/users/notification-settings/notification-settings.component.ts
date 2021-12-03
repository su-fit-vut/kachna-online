// notification-settings.component.ts
// Author: František Nečas

import { Component, OnInit } from '@angular/core';
import { SwPush } from "@angular/service-worker";
import { PushNotificationsService } from "../../shared/services/push-notifications.service";
import { PushConfiguration } from "../../models/users/push-configuration.model";
import { ToastrService } from "ngx-toastr";
import { AuthenticationService } from "../../shared/services/authentication.service";
import { HttpStatusCode } from "@angular/common/http";

@Component({
  selector: 'app-notification-settings',
  templateUrl: './notification-settings.component.html',
  styleUrls: ['./notification-settings.component.css']
})
export class NotificationSettingsComponent implements OnInit {
  subscription: PushSubscription | null = null;
  configuration: PushConfiguration = new PushConfiguration();

  constructor(private swPush: SwPush, private pushService: PushNotificationsService,
              private toastrService: ToastrService, public auth: AuthenticationService) {
    if (swPush.isEnabled) {
      swPush.subscription.subscribe(subscription => {
        this.subscription = subscription;
        if (subscription) {
          this.pushService.getConfiguration(subscription.endpoint).subscribe(configuration => {
            this.configuration = configuration;
          }, err => {
            if (err.status != HttpStatusCode.NotFound) {
              console.log(err);
              this.toastrService.error("Načtení současného nastavení se nezdařilo.");
            }
          })
        }
      })
    }
  }

  ngOnInit(): void {
  }

  subscribe() {
    this.pushService.getPublicKey().subscribe(publicKey => {
      this.swPush.requestSubscription({
        serverPublicKey: publicKey
      }).then(subscription => {
        this.pushService.subscribe(subscription, this.configuration).subscribe(_ => {
            this.toastrService.success("Push notifikace povoleny. Zkontroluj ještě jejich konkrétní nastvení níže.");
          },
          err => {
            console.log(err);
            this.toastrService.error("Nepovedlo se přihlásit k odběru push notifikací.");
          })
      }).catch(err => {
        console.log(err);
        this.toastrService.error("Tvůj prohlížeč zřejmě nepodporuje push notifikace.");
      });
    }, err => {
      console.log(err);
      this.toastrService.error("Nepodařilo se stáhnout veřejný klíč pro push notifikace.");
    });
  }

  unsubscribe() {
    let endpoint = this.subscription?.endpoint || null;
    this.swPush.unsubscribe().then(() => {
      if (endpoint)
        this.pushService.unsubscribe(endpoint).subscribe(() => {
          this.configuration.boardGamesEnabled = false;
          this.configuration.stateChangesEnabled = false;
          this.toastrService.success("Push notifikace vypnuty.");
        }, err => {
          console.log(err);
          this.toastrService.error("Odhlášení od push notifikací se nezdařilo.");
        })
    })
  }

  onStateEnableChange(value: boolean): void {
    this.configuration.stateChangesEnabled = value;
    this.updateBackendSettings();
  }

  onBoardGameEnableChange(value: boolean): void {
    this.configuration.boardGamesEnabled = value;
    this.updateBackendSettings();
  }

  updateBackendSettings() {
    if (this.subscription) {
      this.pushService.subscribe(this.subscription, this.configuration).subscribe(_ => {
        this.toastrService.success("Preference pro push notifikace aktualizovány.");
      }, err => {
        console.log(err);
        this.toastrService.error("Aktualizace preferencí se nezdařila.")
      })
    } else {
      this.toastrService.error("Push notifikace nejsou aktivní.");
    }
  }
}
