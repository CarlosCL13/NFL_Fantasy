import { Observable } from 'rxjs';
import { NflTeam } from '../../shared/models/team.model';

/**
 * Interface que define el contrato para el servicio de equipos NFL
 * Cumple con Interface Segregation Principle (ISP) y Dependency Inversion Principle (DIP)
 */
export interface INflTeamService {
  createTeam(formData: FormData): Observable<NflTeam>;
  getTeams(): Observable<NflTeam[]>;
}
