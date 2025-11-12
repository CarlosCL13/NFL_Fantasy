import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Validadores personalizados para contraseñas
 * Reutilizable en múltiples componentes
 */
export class PasswordValidators {
  /**
   * Valida que las contraseñas coincidan entre dos campos
   * @param passwordField - Nombre del campo de contraseña
   * @param confirmPasswordField - Nombre del campo de confirmación
   * @returns ValidatorFn para uso en FormGroup
   */
  static matchPassword(passwordField: string, confirmPasswordField: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const password = control.get(passwordField);
      const confirmPassword = control.get(confirmPasswordField);

      if (!password || !confirmPassword) {
        return null;
      }

      if (password.value !== confirmPassword.value) {
        confirmPassword.setErrors({ ...confirmPassword.errors, passwordMismatch: true });
        return { passwordMismatch: true };
      } else {
        // Limpiar el error de passwordMismatch si coinciden
        const errors = confirmPassword.errors;
        if (errors) {
          delete errors['passwordMismatch'];
          confirmPassword.setErrors(Object.keys(errors).length > 0 ? errors : null);
        }
      }

      return null;
    };
  }

  /**
   * Valida que la contraseña cumpla con el formato requerido
   * - Entre 8 y 12 caracteres
   * - Al menos una minúscula
   * - Al menos una mayúscula
   * - Al menos un número
   * @returns ValidatorFn
   */
  static securePassword(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;

      if (!value) {
        return null;
      }

      const hasMinLength = value.length >= 8;
      const hasMaxLength = value.length <= 12;
      const hasLowerCase = /[a-z]/.test(value);
      const hasUpperCase = /[A-Z]/.test(value);
      const hasNumber = /[0-9]/.test(value);

      const valid = hasMinLength && hasMaxLength && hasLowerCase && hasUpperCase && hasNumber;

      if (!valid) {
        return {
          securePassword: {
            hasMinLength,
            hasMaxLength,
            hasLowerCase,
            hasUpperCase,
            hasNumber
          }
        };
      }

      return null;
    };
  }
}
