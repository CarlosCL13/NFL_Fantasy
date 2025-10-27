import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateSeasonDto, Season } from '../../shared/models/season.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SeasonService {
  private readonly apiUrl = `${environment.apiUrl}/api/seasons`;

  constructor(private httpClient: HttpClient) { }

  /**
   * Crea una nueva temporada en el sistema
   * @param createSeasonDto - Datos para crear la temporada
   * @returns Observable con la respuesta del servidor
   */
  createSeason(createSeasonDto: CreateSeasonDto): Observable<any> {
    return this.httpClient.post(this.apiUrl, createSeasonDto);
  }

  /**
   * Obtiene todas las temporadas disponibles
   * @returns Observable con la lista de temporadas
   */
  getAllSeasons(): Observable<Season[]> {
    return this.httpClient.get<Season[]>(this.apiUrl);
  }

  /**
   * Verifica si un nombre de temporada está disponible
   * @param name - Nombre a verificar
   * @returns Observable con información de disponibilidad
   */
  checkNameAvailability(name: string): Observable<any> {
    return this.httpClient.get(`${this.apiUrl}/check-name/${encodeURIComponent(name)}`);
  }

  /**
   * Obtiene la temporada actual activa
   * @returns Observable con la temporada actual
   */
  getCurrentSeason(): Observable<Season> {
    return this.httpClient.get<Season>(`${this.apiUrl}/current`);
  }

  /**
   * Verifica conflictos potenciales antes de crear una temporada
   * @param createSeasonDto - Datos de la temporada a verificar
   * @returns Observable con información de conflictos
   */
  checkConflicts(createSeasonDto: CreateSeasonDto): Observable<any> {
    return this.httpClient.post(`${this.apiUrl}/check-conflicts`, createSeasonDto);
  }
}