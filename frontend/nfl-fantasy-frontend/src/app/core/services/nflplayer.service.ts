import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { INflPlayerService } from '../interfaces/nflplayer-service.interface';

/**
 * Servicio para gestionar operaciones relacionadas con jugadores NFL
 * Implementa INflPlayerService para cumplir con Dependency Inversion Principle (DIP)
 */
@Injectable({ providedIn: 'root' })
export class NflPlayerService implements INflPlayerService {
  private api = `${environment.apiUrl}/api/nflplayers`;

  constructor(private http: HttpClient) {}

  createPlayer(formData: FormData): Observable<any> {
    return this.http.post(this.api, formData);
  }

  bulkUpload(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('File', file);
    return this.http.post(`${this.api}/bulk-upload`, formData);
  }

  getPositions(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/api/Position`);
  }
}
