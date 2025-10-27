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

@Injectable({
  providedIn: 'root',
})
export class LeagueService {
  private readonly apiUrl = `${environment.apiUrl}/api/leagues`;

  constructor(private httpClient: HttpClient) {}

  /**
   * Crea una nueva liga en el sistema
   * @param createLeagueDto - Datos para crear la liga
   * @returns Observable con la respuesta del servidor
   */
  createLeague(createLeagueDto: CreateLeagueDto): Observable<CreateLeagueResponse> {
    return this.httpClient.post<CreateLeagueResponse>(this.apiUrl, createLeagueDto);
  }

  /**
   * Verifica si un nombre de liga está disponible mediante búsqueda
   * @param name - Nombre a verificar
   * @returns Observable con información de disponibilidad
   */
  checkNameAvailability(name: string): Observable<NameAvailabilityResponse> {
    // Simulamos la verificación usando el endpoint de búsqueda
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

  /**
   * Busca ligas con filtros opcionales
   * @param filters - Filtros de búsqueda
   * @returns Observable con la lista de ligas
   */
  searchLeagues(filters: any): Observable<League[]> {
    return this.httpClient.get<League[]>(`${this.apiUrl}/search`, { params: filters });
  }

  /**
   * Obtiene todas las ligas disponibles
   * @returns Observable con la lista de ligas
   */
  getAllLeagues(): Observable<League[]> {
    return this.httpClient.get<League[]>(this.apiUrl);
  }
}
