import { Observable } from 'rxjs';
import { CreateSeasonDto, Season } from '../../shared/models/season.model';

/**
 * Interface que define el contrato para el servicio de temporadas
 * Cumple con Interface Segregation Principle (ISP) y Dependency Inversion Principle (DIP)
 */
export interface ISeasonService {
  createSeason(createSeasonDto: CreateSeasonDto): Observable<any>;
  getAllSeasons(): Observable<Season[]>;
  checkNameAvailability(name: string): Observable<any>;
  getCurrentSeason(): Observable<Season>;
  checkConflicts(createSeasonDto: CreateSeasonDto): Observable<any>;
}
