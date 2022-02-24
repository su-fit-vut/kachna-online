import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Image } from "../../models/images/image.model";
import { environment } from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class ImageUploadService {

  constructor(private http: HttpClient) { }

  readonly ImagesUrl = environment.baseApiUrl + '/images';

  postFile(file: File): Observable<Image> {
    const formData: FormData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post<Image>(this.ImagesUrl, formData);
  }
}
