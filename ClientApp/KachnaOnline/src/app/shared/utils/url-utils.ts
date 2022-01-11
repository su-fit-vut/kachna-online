import { environment } from "../../../environments/environment";

export class UrlUtils {
  public static getAbsoluteImageUrl(url: string): string {
    if (url.startsWith('https://')) {
      return url;
    }

    return `https://${environment.baseApiUrlDomain}${url}`;
  }
}
