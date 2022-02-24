/**
 * KIS information about the logged in user.
 */
export class KisLoggedInUserInformation {
  email: string = "";
  gamification_consent: boolean = false;
  has_eduid: boolean = false;
  has_rfid: boolean = false;
  id: number = -1;
  is_member: boolean = false;
  member_until: string = "";
  name: string = "";
  nickname: string = "";
  roles: string[];
  contributions: Contributions;
  orders: Orders;
  pin: string = "";
  prestige: number = -1;
}

export class Contributions {
  month: number = -1;
  year: number = -1;
}

export class Orders {
  month: number = -1;
  year: number = -1;
}
