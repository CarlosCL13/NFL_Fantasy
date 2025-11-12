import { Injectable } from '@angular/core';
import { AbstractControl } from '@angular/forms';

/**
 * Servicio para generar mensajes de error de validación de formularios
 * Cumple con Single Responsibility Principle (SRP)
 * Cumple con Open/Closed Principle (OCP) - se puede extender agregando nuevos métodos sin modificar los existentes
 */
@Injectable({
  providedIn: 'root'
})
export class FormValidationMessageService {
  
  /**
   * Obtiene el mensaje de error para un control de formulario
   * @param control - Control a validar
   * @param fieldName - Nombre del campo (para mensajes personalizados)
   * @returns Mensaje de error o null si no hay errores
   */
  getErrorMessage(control: AbstractControl | null, fieldName: string): string | null {
    if (!control || !control.errors || !control.touched) {
      return null;
    }

    const errors = control.errors;

    if (errors['required']) {
      return this.getRequiredMessage(fieldName);
    }

    if (errors['email']) {
      return 'Por favor, ingrese un correo electrónico válido';
    }

    if (errors['minlength']) {
      return `${fieldName} debe tener al menos ${errors['minlength'].requiredLength} caracteres`;
    }

    if (errors['maxlength']) {
      return `${fieldName} no puede exceder ${errors['maxlength'].requiredLength} caracteres`;
    }

    if (errors['min']) {
      return `${fieldName} debe ser al menos ${errors['min'].min}`;
    }

    if (errors['max']) {
      return `${fieldName} no puede ser mayor a ${errors['max'].max}`;
    }

    if (errors['passwordMismatch']) {
      return 'Las contraseñas no coinciden';
    }

    if (errors['securePassword']) {
      return this.getSecurePasswordMessage(errors['securePassword']);
    }

    if (errors['endDateBeforeStart']) {
      return 'La fecha de fin debe ser posterior a la fecha de inicio';
    }

    if (errors['dateInPast']) {
      return 'La fecha no puede ser anterior a hoy';
    }

    if (errors['dateRange']) {
      return `La fecha debe estar entre ${errors['dateRange'].min} y ${errors['dateRange'].max}`;
    }

    return 'Campo inválido';
  }

  /**
   * Obtiene mensaje personalizado para campo requerido
   * @param fieldName - Nombre del campo
   * @returns Mensaje de error
   */
  private getRequiredMessage(fieldName: string): string {
    const messages: { [key: string]: string } = {
      'email': 'El correo electrónico es obligatorio',
      'password': 'La contraseña es obligatoria',
      'confirmPassword': 'Debe confirmar la contraseña',
      'name': 'El nombre es obligatorio',
      'startDate': 'La fecha de inicio es obligatoria',
      'endDate': 'La fecha de fin es obligatoria',
      'weeksCount': 'El número de semanas es obligatorio',
    };

    return messages[fieldName.toLowerCase()] || `El campo ${fieldName} es obligatorio`;
  }

  /**
   * Obtiene mensaje detallado para contraseña insegura
   * @param errors - Objeto con detalles de validación
   * @returns Mensaje de error detallado
   */
  private getSecurePasswordMessage(errors: any): string {
    const requirements = [];
    
    if (!errors.hasMinLength) {
      requirements.push('al menos 8 caracteres');
    }
    if (!errors.hasMaxLength) {
      requirements.push('máximo 12 caracteres');
    }
    if (!errors.hasLowerCase) {
      requirements.push('una letra minúscula');
    }
    if (!errors.hasUpperCase) {
      requirements.push('una letra mayúscula');
    }
    if (!errors.hasNumber) {
      requirements.push('un número');
    }

    return `La contraseña debe tener ${requirements.join(', ')}`;
  }

  /**
   * Verifica si un campo tiene errores
   * @param control - Control a verificar
   * @returns true si tiene errores y fue tocado
   */
  hasError(control: AbstractControl | null): boolean {
    return !!(control && control.invalid && control.touched);
  }

  /**
   * Obtiene un mapa de nombres de campo amigables
   * Se puede extender para más campos
   */
  private getFieldDisplayNames(): { [key: string]: string } {
    return {
      'email': 'Correo electrónico',
      'password': 'Contraseña',
      'confirmPassword': 'Confirmación de contraseña',
      'name': 'Nombre',
      'description': 'Descripción',
      'startDate': 'Fecha de inicio',
      'endDate': 'Fecha de fin',
      'weeksCount': 'Número de semanas',
      'maxTeams': 'Cantidad de equipos',
      'commissionerTeamName': 'Nombre del equipo',
    };
  }

  /**
   * Obtiene el nombre de visualización amigable para un campo
   * @param fieldName - Nombre técnico del campo
   * @returns Nombre amigable
   */
  getDisplayName(fieldName: string): string {
    const displayNames = this.getFieldDisplayNames();
    return displayNames[fieldName] || fieldName;
  }
}
