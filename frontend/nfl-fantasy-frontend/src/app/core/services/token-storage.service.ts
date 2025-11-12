import { Injectable } from '@angular/core';

/**
 * Interface que define el contrato para el almacenamiento de tokens
 */
export interface ITokenStorage {
  getToken(): string | null;
  setToken(token: string): void;
  removeToken(): void;
  getUserData(): any | null;
  setUserData(user: any): void;
  removeUserData(): void;
  clear(): void;
}

/**
 * Servicio responsable únicamente del almacenamiento y recuperación de tokens y datos de usuario
 * Cumple con Single Responsibility Principle (SRP)
 * Implementa una interfaz para cumplir con Dependency Inversion Principle (DIP)
 */
@Injectable({
  providedIn: 'root'
})
export class TokenStorageService implements ITokenStorage {
  private readonly TOKEN_KEY = 'token';
  private readonly USER_KEY = 'user';

  /**
   * Obtiene el token de autenticación almacenado
   * @returns El token JWT o null si no existe
   */
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  /**
   * Almacena el token de autenticación
   * @param token - Token JWT a almacenar
   */
  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  /**
   * Elimina el token de autenticación del almacenamiento
   */
  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  /**
   * Obtiene los datos del usuario almacenados
   * @returns Objeto con datos del usuario o null si no existe
   */
  getUserData(): any | null {
    const user = localStorage.getItem(this.USER_KEY);
    if (!user) return null;
    
    try {
      return JSON.parse(user);
    } catch (error) {
      console.error('Error parsing user data:', error);
      return null;
    }
  }

  /**
   * Almacena los datos del usuario
   * @param user - Objeto con datos del usuario
   */
  setUserData(user: any): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  /**
   * Elimina los datos del usuario del almacenamiento
   */
  removeUserData(): void {
    localStorage.removeItem(this.USER_KEY);
  }

  /**
   * Limpia todos los datos de autenticación
   */
  clear(): void {
    this.removeToken();
    this.removeUserData();
  }
}
