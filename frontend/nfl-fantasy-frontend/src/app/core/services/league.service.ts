import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import {
  CreateLeagueDto,
  League,
  CreateLeagueResponse,
  NameAvailabilityResponse,
} from '../../shared/models/league.model';
import { environment } from '../../../environments/environment';
import { ILeagueService } from '../interfaces/league-service.interface';

/**
 * Servicio para gestionar operaciones relacionadas con ligas
 * Implementa ILeagueService para cumplir con Dependency Inversion Principle (DIP)
 */
@Injectable({
  providedIn: 'root',
})
export class LeagueService implements ILeagueService {
  private readonly apiUrl = `${environment.apiUrl}/api/leagues`;

  constructor(private httpClient: HttpClient) {}

  createLeague(createLeagueDto: CreateLeagueDto): Observable<CreateLeagueResponse> {
    return this.httpClient.post<CreateLeagueResponse>(this.apiUrl, createLeagueDto);
  }

  checkNameAvailability(name: string): Observable<NameAvailabilityResponse> {
    return this.httpClient
      .get<League[]>(`${this.apiUrl}/search?name=${encodeURIComponent(name)}`)
      .pipe(
        map((leagues: League[]) => {
          const exactMatch = leagues.find(
            (league) => league.name.toLowerCase() === name.toLowerCase()
          );
          return {
            isAvailable: !exactMatch,
            message: exactMatch ? 'Este nombre ya está en uso' : 'Nombre disponible',
          };
        })
      );
  }

  searchLeagues(filters: any): Observable<League[]> {
    return this.httpClient.get<League[]>(`${this.apiUrl}/search`, { params: filters });
  }

  getAllLeagues(): Observable<League[]> {
    return this.httpClient.get<League[]>(this.apiUrl);
  }

  /** 🆕 Unirse a una liga existente (requiere token) */
  joinLeague(data: {
    leagueId: number;
    password: string;
    alias: string;
    teamName: string;
  }): Observable<any> {
    return this.httpClient.post(`${this.apiUrl}/join`, data);
  }
}
