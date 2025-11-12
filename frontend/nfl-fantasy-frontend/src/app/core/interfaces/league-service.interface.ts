import { Observable } from 'rxjs';
import { CreateLeagueDto, League, CreateLeagueResponse, NameAvailabilityResponse } from '../../shared/models/league.model';

/**
 * Interface que define el contrato para el servicio de ligas
 * Cumple con Interface Segregation Principle (ISP) y Dependency Inversion Principle (DIP)
 * Permite cambiar la implementación sin afectar los componentes
 */
export interface ILeagueService {
  createLeague(createLeagueDto: CreateLeagueDto): Observable<CreateLeagueResponse>;
  checkNameAvailability(name: string): Observable<NameAvailabilityResponse>;
  searchLeagues(filters: any): Observable<League[]>;
  getAllLeagues(): Observable<League[]>;
  joinLeague(data: { leagueId: number; password: string; alias: string; teamName: string }): Observable<any>;
}
