import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class NflPlayerService {
  private api = `${environment.apiUrl}/api/nflplayers`;

  constructor(private http: HttpClient) {}

  createPlayer(formData: FormData): Observable<any> {
    return this.http.post(this.api, formData);
  }

  bulkUpload(file: File) {
    const formData = new FormData();
    formData.append('File', file);
    return this.http.post(`${this.api}/bulk-upload`, formData);
  }

  getPositions(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/api/Position`);
  }
}
