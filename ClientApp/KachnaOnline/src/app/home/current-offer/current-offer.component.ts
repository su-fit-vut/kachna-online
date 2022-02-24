import { Component, OnInit } from '@angular/core';
import { ClubInfoService } from "../../shared/services/club-info.service";
import { ClubOffer, ClubOfferItem } from "../../models/clubinfo/offer.model";

@Component({
  selector: 'app-current-offer',
  templateUrl: './current-offer.component.html',
  styleUrls: ['./current-offer.component.css']
})
export class CurrentOfferComponent implements OnInit {

  constructor(private infoService: ClubInfoService) { }

  loaded: boolean = false;
  hasError: boolean = false;
  hasTaps: boolean = false;
  hasProducts: boolean = false;

  categories: string[] = [];
  products: { [categoryName: string]: ClubOfferItem[] } = {};

  taps: ClubOfferItem[];

  ngOnInit(): void {
    this.loadOffer();
  }

  loadOffer() {
    this.loaded = false;
    this.hasError = false;
    this.hasTaps = false;
    this.hasProducts = false;

    this.infoService.getOffer().subscribe(res => this.showOffer(res), _ => this.hasError = true);
  }

  showOffer(offer: ClubOffer) {
    this.loaded = true;

    if (offer.beersOnTap?.length > 0) {
      this.hasTaps = true;
      this.taps = offer.beersOnTap;
    }

    if (offer.products?.length > 0) {
      this.hasProducts = true;

      let coffee = [];
      let bev = [];
      let food = [];
      let others = [];

      for (let p of offer.products) {
        if (p.labels.includes("Káva")) {
          coffee.push(p);
        } else if (p.labels.includes("Nealko")) {
          bev.push(p);
        } else if (p.labels.includes("Jídlo")) {
          food.push(p);
        } else {
          others.push(p);
        }
      }

      this.makeCategories({"Káva": coffee, "Nealko": bev, "Jídlo": food, "Další nabídka": others});
    }
  }

  makeCategories(definitions: { [id: string]: ClubOfferItem[] }) {
    for (let [key, value] of Object.entries(definitions)) {
      if (value.length > 0) {
        this.categories.push(key);
        this.products[key] = value;
      }
    }
  }
}
