import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { LoginDto } from '../../shared/models/login.dto';
import { Observable, tap } from 'rxjs';
import { TokenStorageService } from './token-storage.service';

/**
 * Interface que define el contrato para el servicio de autenticación
 */
export interface IAuthService {
  register(formData: FormData): Observable<any>;
  login(dto: LoginDto): Observable<any>;
  logout(): void;
  isAuthenticated(): boolean;
  getUserRole(): string | null;
  getUserName(): string | null;
}

/**
 * Servicio responsable únicamente de la lógica de autenticación
 * Cumple con Single Responsibility Principle (SRP)
 * Delega el almacenamiento a TokenStorageService 
 * ya NO maneja navegación (se delega a los componentes)
 */
@Injectable({ providedIn: 'root' })
export class AuthService implements IAuthService {
  private api = `${environment.apiUrl}/api/users`;

  constructor(
    private http: HttpClient,
    private tokenStorage: TokenStorageService
  ) {}

  /**
   * Registra un nuevo usuario en el sistema
   * @param formData - Datos del formulario de registro
   * @returns Observable con la respuesta del servidor
   */
  register(formData: FormData): Observable<any> {
    return this.http.post(`${this.api}/register`, formData);
  }

  /**
   * Inicia sesión y almacena el token y datos del usuario
   * @param dto - Credenciales de inicio de sesión
   * @returns Observable con la respuesta del servidor
   */
  login(dto: LoginDto): Observable<any> {
    return this.http.post(`${this.api}/login`, dto).pipe(
      tap((res: any) => {
        if (res?.token) {
          this.tokenStorage.setToken(res.token);
          this.tokenStorage.setUserData(res.user || {});
        }
      })
    );
  }

  /**
   * Cierra la sesión del usuario eliminando sus datos
   * La navegación debe ser manejada por el componente que llama este método
   */
  logout(): void {
    this.tokenStorage.clear();
  }

  /**
   * Obtiene el token de autenticación
   * @returns El token JWT o null si no existe
   */
  getToken(): string | null {
    return this.tokenStorage.getToken();
  }

  /**
   * Verifica si el usuario está autenticado
   * @returns true si hay un token válido, false en caso contrario
   */
  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  /**
   * Obtiene el rol del usuario autenticado
   * @returns El rol del usuario o null si no está disponible
   */
  getUserRole(): string | null {
    const user = this.tokenStorage.getUserData();
    return user?.role || null;
  }

  /**
   * Obtiene el nombre del usuario autenticado
   * @returns El nombre del usuario o null si no está disponible
   */
  getUserName(): string | null {
    const user = this.tokenStorage.getUserData();
    return user?.name || null;
  }
}
