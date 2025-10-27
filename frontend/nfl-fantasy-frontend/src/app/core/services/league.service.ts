import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class LeagueService {
  private baseUrl = `${environment.apiUrl}/api`;

  constructor(private http: HttpClient) {}

  // Obtener todas las temporadas
  getSeasons(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/seasons`);
  }

  // Buscar ligas según SeasonId (opcionalmente por nombre)
  searchLeagues(seasonId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/leagues/search`, {
      params: { SeasonId: seasonId.toString() },
    });
  }

  // Unirse a una liga
  joinLeague(data: any): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    });

    return this.http.post(`${this.baseUrl}/leagues/join`, data, { headers });
  }
}
