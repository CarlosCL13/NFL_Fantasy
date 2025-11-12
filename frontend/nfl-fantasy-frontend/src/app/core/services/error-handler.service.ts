import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

/**
 * Interface para definir manejadores de error específicos
 * Permite extensión sin modificación (Open/Closed Principle)
 */
export interface IErrorHandler {
  canHandle(error: HttpErrorResponse): boolean;
  handle(error: HttpErrorResponse): string;
}

/**
 * Manejador de errores de validación (400)
 */
class ValidationErrorHandler implements IErrorHandler {
  private errorMessages: { [key: string]: string } = {
    'Ya existe una liga con ese nombre': 'El nombre de la liga ya está en uso. Por favor, elija un nombre diferente y único para su liga.',
    'contraseña no cumple el formato': 'La contraseña no cumple los requisitos de seguridad. Debe tener entre 8-12 caracteres, ser alfanumérica y contener al menos una minúscula, una mayúscula y un número.',
    'cantidad de equipos no es válida': 'La cantidad de equipos seleccionada no es válida. Debe seleccionar entre 4, 6, 8, 10, 12, 14, 16, 18 o 20 equipos.',
    'No hay una temporada actual activa': 'No se puede crear la liga porque no hay una temporada activa. Contacte al administrador del sistema.',
    'El alias del equipo ya existe': 'El nombre del equipo que eligió genera un alias que ya está en uso. Por favor, elija un nombre diferente para su equipo.',
    'nombre de equipo ya existe': 'El nombre del equipo ya está en uso en esta liga. Por favor, elija un nombre diferente para su equipo.'
  };

  canHandle(error: HttpErrorResponse): boolean {
    return error.status === 400;
  }

  handle(error: HttpErrorResponse): string {
    if (error.error && typeof error.error === 'object') {
      const backendError = error.error.error || error.error.message;
      
      // Buscar coincidencia en mensajes conocidos
      for (const [key, message] of Object.entries(this.errorMessages)) {
        if (backendError?.includes(key)) {
          return message;
        }
      }
      
      return `Error de validación: ${backendError}`;
    } else if (typeof error.error === 'string') {
      return `Error de validación: ${error.error}`;
    }
    
    return 'Los datos ingresados no son válidos. Por favor, revise todos los campos y corrija los errores.';
  }
}

/**
 * Manejador de errores de autenticación (401)
 */
class AuthenticationErrorHandler implements IErrorHandler {
  canHandle(error: HttpErrorResponse): boolean {
    return error.status === 401;
  }

  handle(error: HttpErrorResponse): string {
    return 'Su sesión ha expirado. Por favor, inicie sesión nuevamente.';
  }
}

/**
 * Manejador de errores de autorización (403)
 */
class AuthorizationErrorHandler implements IErrorHandler {
  canHandle(error: HttpErrorResponse): boolean {
    return error.status === 403;
  }

  handle(error: HttpErrorResponse): string {
    return 'No tiene permisos para realizar esta acción. Contacte al administrador del sistema.';
  }
}

/**
 * Manejador de errores del servidor (500)
 */
class ServerErrorHandler implements IErrorHandler {
  canHandle(error: HttpErrorResponse): boolean {
    return error.status === 500;
  }

  handle(error: HttpErrorResponse): string {
    return 'Error interno del servidor. Por favor, intente nuevamente en unos minutos. Si el problema persiste, contacte al soporte técnico.';
  }
}

/**
 * Manejador de errores de conexión
 */
class NetworkErrorHandler implements IErrorHandler {
  canHandle(error: HttpErrorResponse): boolean {
    return error.status === 0 || !navigator.onLine;
  }

  handle(error: HttpErrorResponse): string {
    return 'No se pudo conectar con el servidor. Verifique su conexión a internet e intente nuevamente.';
  }
}

/**
 * Manejador por defecto para errores no contemplados
 */
class DefaultErrorHandler implements IErrorHandler {
  canHandle(error: HttpErrorResponse): boolean {
    return true; // Siempre puede manejar como último recurso
  }

  handle(error: HttpErrorResponse): string {
    return 'Ocurrió un error inesperado. Por favor, intente nuevamente. Si el problema persiste, contacte al soporte técnico.';
  }
}

/**
 * Servicio centralizado para manejar errores HTTP
 * Cumple con Open/Closed Principle: abierto para extensión (agregar nuevos handlers), cerrado para modificación
 * Cumple con Single Responsibility Principle: solo maneja errores
 */
@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
  private errorHandlers: IErrorHandler[];

  constructor() {
    // Chain of Responsibility Pattern
    this.errorHandlers = [
      new ValidationErrorHandler(),
      new AuthenticationErrorHandler(),
      new AuthorizationErrorHandler(),
      new ServerErrorHandler(),
      new NetworkErrorHandler(),
      new DefaultErrorHandler() // Siempre al final
    ];
  }

  /**
   * Maneja un error HTTP y retorna un mensaje amigable
   * @param error - Error HTTP recibido
   * @param context - Contexto opcional para mensajes más específicos
   * @returns Mensaje de error amigable para mostrar al usuario
   */
  handleError(error: HttpErrorResponse, context?: string): string {
    const handler = this.errorHandlers.find(h => h.canHandle(error));
    const message = handler ? handler.handle(error) : 'Error desconocido';
    
    // Log del error para debugging
    console.error(`Error${context ? ` en ${context}` : ''}:`, error);
    
    return message;
  }

  /**
   * Permite agregar un manejador de error personalizado
   * Cumple con Open/Closed Principle
   * @param handler - Nuevo manejador de error
   * @param position - Posición en la cadena (por defecto antes del DefaultErrorHandler)
   */
  addErrorHandler(handler: IErrorHandler, position?: number): void {
    if (position !== undefined) {
      this.errorHandlers.splice(position, 0, handler);
    } else {
      // Agregar antes del último (DefaultErrorHandler)
      this.errorHandlers.splice(this.errorHandlers.length - 1, 0, handler);
    }
  }
}
