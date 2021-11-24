export class LocalTokenContent {
  sub: string = ""; // FIXME: Change to number later.
  email: string = "";
  given_name: string = "";
  krt: string = "";
  role: string[] = [];
  nbf: number = -1;
  exp: number = -1;
  iat: number = -1;
}
