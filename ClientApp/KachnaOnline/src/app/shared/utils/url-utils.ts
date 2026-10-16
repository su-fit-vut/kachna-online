import { environment } from "../../../environments/environment";

export class UrlUtils {
  public static getAbsoluteImageUrl(url: string): string {
    if (url.startsWith('https://') || url.startsWith('http://')) {
      return url;
    }

    return `${window.location.protocol}//${environment.baseApiUrlDomain}${url}`;
  }
}
