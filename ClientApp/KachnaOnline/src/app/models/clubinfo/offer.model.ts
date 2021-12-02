// offer.model.ts
// Author: Ondřej Ondryáš

export class ClubOfferItem {
  name: string;
  price: number;
  prestige: number;
  imageUrl: string | null;
  isPermanentOffer: boolean;
  labels: string[];
}

export class ClubOffer {
  products: ClubOfferItem[];
  beersOnTap: ClubOfferItem[];
}
