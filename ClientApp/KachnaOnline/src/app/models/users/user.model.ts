// user.model.ts
// Author: David Chocholatý

export class User {
  id: number = -1;
  name: string = "";
  email: string = "";
  nickname: string = "";
  gamificationConsent: boolean = false;
  cardCode: string = "";
  prestige: number = -1;
}

export class UserDetail {
  id: number = -1;
  name: string = "";
  nickname: string = "";
  email: string = "";
  discordId: string = "";
  activeRoles: string[] = [];
}
