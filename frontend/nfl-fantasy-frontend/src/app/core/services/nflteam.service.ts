import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { NflTeam } from '../../shared/models/team.model';
import { Observable } from 'rxjs';
import { INflTeamService } from '../interfaces/nflteam-service.interface';

/**
 * Servicio para gestionar operaciones relacionadas con equipos NFL
 * Implementa INflTeamService para cumplir con Dependency Inversion Principle (DIP)
 */
@Injectable({ providedIn: 'root' })
export class NflTeamService implements INflTeamService {
  private api = `${environment.apiUrl}/api/nflteams`;

  constructor(private http: HttpClient) {}

  createTeam(formData: FormData): Observable<NflTeam> {
    return this.http.post<NflTeam>(this.api, formData);
  }

  getTeams(): Observable<NflTeam[]> {
    return this.http.get<NflTeam[]>(this.api);
  }
}
