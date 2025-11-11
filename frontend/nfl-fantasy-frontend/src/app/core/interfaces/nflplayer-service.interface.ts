import { Observable } from 'rxjs';

/**
 * Interface que define el contrato para el servicio de jugadores NFL
 * Cumple con Interface Segregation Principle (ISP) y Dependency Inversion Principle (DIP)
 */
export interface INflPlayerService {
  createPlayer(formData: FormData): Observable<any>;
  bulkUpload(file: File): Observable<any>;
  getPositions(): Observable<any[]>;
}
