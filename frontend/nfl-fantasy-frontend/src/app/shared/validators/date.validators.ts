import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Validadores personalizados para fechas
 * Cumple con Single Responsibility Principle (SRP)
 * Reutilizable en múltiples componentes
 */
export class DateValidators {
  /**
   * Valida que la fecha de fin sea posterior a la fecha de inicio
   * @param startDateField - Nombre del campo de fecha de inicio
   * @param endDateField - Nombre del campo de fecha de fin
   * @returns ValidatorFn para uso en FormGroup
   */
  static endDateAfterStartDate(startDateField: string, endDateField: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const startDate = control.get(startDateField);
      const endDate = control.get(endDateField);

      if (!startDate || !endDate || !startDate.value || !endDate.value) {
        return null;
      }

      const start = new Date(startDate.value);
      const end = new Date(endDate.value);

      if (end <= start) {
        endDate.setErrors({ ...endDate.errors, endDateBeforeStart: true });
        return { endDateBeforeStart: true };
      } else {
        // Limpiar el error si la validación pasa
        const errors = endDate.errors;
        if (errors) {
          delete errors['endDateBeforeStart'];
          endDate.setErrors(Object.keys(errors).length > 0 ? errors : null);
        }
      }

      return null;
    };
  }

  /**
   * Valida que la fecha no sea anterior a hoy
   * @returns ValidatorFn
   */
  static notInPast(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }

      const inputDate = new Date(control.value);
      const today = new Date();
      today.setHours(0, 0, 0, 0);

      if (inputDate < today) {
        return { dateInPast: true };
      }

      return null;
    };
  }

  /**
   * Valida que la fecha esté dentro de un rango específico
   * @param minDate - Fecha mínima permitida
   * @param maxDate - Fecha máxima permitida
   * @returns ValidatorFn
   */
  static dateRange(minDate: Date, maxDate: Date): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }

      const inputDate = new Date(control.value);

      if (inputDate < minDate || inputDate > maxDate) {
        return {
          dateRange: {
            min: minDate,
            max: maxDate,
            actual: inputDate
          }
        };
      }

      return null;
    };
  }
}
