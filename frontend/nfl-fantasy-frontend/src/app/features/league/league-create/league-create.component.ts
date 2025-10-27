import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { LeagueService } from '../../../core/services/league.service';
import { CreateLeagueDto } from '../../../shared/models/league.model';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-league-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './league-create.component.html',
  styleUrl: './league-create.component.scss'
})
export class LeagueCreateComponent {
  leagueForm: FormGroup;
  isSubmitting = false;
  aliasPreview = '';
  isCheckingName = false;
  nameAvailable = true;
  nameCheckMessage = '';
  
  // Opciones para cantidad de equipos
  teamCountOptions = [4, 6, 8, 10, 12, 14, 16, 18, 20];
  
  // Opciones para tipo de playoffs
  playoffOptions = [
    { value: 4, label: '4 equipos (semanas 16 y 17)' },
    { value: 6, label: '6 equipos (semanas 16, 17 y 18)' }
  ];

  constructor(
    private formBuilder: FormBuilder,
    private leagueService: LeagueService,
    public router: Router
  ) {
    this.leagueForm = this.createLeagueForm();
    this.setupNameValidation();
  }

  /**
   * Crea el formulario reactivo para la liga con validaciones
   * @returns FormGroup configurado para crear liga
   */
  private createLeagueForm(): FormGroup {
    return this.formBuilder.group({
      name: ['', [
        Validators.required, 
        Validators.minLength(1), 
        Validators.maxLength(100)
      ]],
      description: ['', [
        Validators.maxLength(500)
      ]],
      maxTeams: [8, [
        Validators.required
      ]],
      password: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(12),
        Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,12}$')
      ]],
      confirmPassword: ['', [
        Validators.required
      ]],
      playoffType: [4, [
        Validators.required
      ]],
      commissionerTeamName: ['', [
        Validators.required,
        Validators.minLength(1),
        Validators.maxLength(100)
      ]]
    }, { validators: this.passwordMatchValidator });
  }

  /**
   * Validador personalizado para confirmar que las contraseñas coincidan
   * @param form - FormGroup a validar
   * @returns objeto con error si no coinciden, null si coinciden
   */
  private passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (!password || !confirmPassword) {
      return null;
    }
    
    return password.value === confirmPassword.value ? null : { passwordMismatch: true };
  }

  /**
   * Maneja el cambio en la selección de cantidad de equipos
   * @param event - Evento del select
   */
  onTeamCountChange(event: any): void {
    const value = parseInt(event.target.value, 10);
    this.leagueForm.patchValue({ maxTeams: value });
  }



  /**
   * Maneja el envío del formulario para crear la liga
   */
  onSubmit(): void {
    if (this.leagueForm.invalid) {
      this.markFormGroupTouched();
      alert('Por favor, complete todos los campos obligatorios y corrija los errores antes de crear la liga.');
      return;
    }



    if (!this.nameAvailable) {
      alert('El nombre ingresado para la liga ya está en uso. Por favor, elija un nombre único y diferente para su liga.');
      return;
    }

    this.isSubmitting = true;
    
    const createLeagueDto: CreateLeagueDto = {
      name: this.leagueForm.get('name')?.value,
      description: this.leagueForm.get('description')?.value || undefined,
      maxTeams: this.leagueForm.get('maxTeams')?.value,
      password: this.leagueForm.get('password')?.value,
      playoffType: this.leagueForm.get('playoffType')?.value,
      commissionerTeamName: this.leagueForm.get('commissionerTeamName')?.value,
      commissionerAlias: this.generateAlias(this.leagueForm.get('commissionerTeamName')?.value)
    };

    this.leagueService.createLeague(createLeagueDto).subscribe({
      next: (response) => {
        const successMessage = `¡Liga "${createLeagueDto.name}" creada exitosamente!\n\n` +
          `• Usted ha sido asignado como comisionado principal\n` +
          `• Su equipo "${createLeagueDto.commissionerTeamName}" ha sido registrado\n` +
          `• Cupos disponibles restantes: ${response.remainingSpots} de ${createLeagueDto.maxTeams}\n` +
          `• Estado inicial: Pre-Draft\n` +
          `• Playoffs configurados para ${createLeagueDto.playoffType} equipos\n\n` +
          `¡Su liga está lista para recibir participantes!`;
        
        alert(successMessage);
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        console.error('Error al crear liga:', error);
        this.handleError(error);
        this.isSubmitting = false;
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  /**
   * Maneja los errores del servidor de manera amigable
   * @param error - Error recibido del servidor
   */
  private handleError(error: any): void {
    let errorMessage = '';
    
    if (error.status === 400) {
      // Errores de validación del backend
      if (error.error && typeof error.error === 'object') {
        const backendError = error.error.error || error.error.message;
        
        if (backendError?.includes('Ya existe una liga con ese nombre')) {
          errorMessage = 'El nombre de la liga ya está en uso. Por favor, elija un nombre diferente y único para su liga.';
        } else if (backendError?.includes('contraseña no cumple el formato')) {
          errorMessage = 'La contraseña no cumple los requisitos de seguridad. Debe tener entre 8-12 caracteres, ser alfanumérica y contener al menos una minúscula, una mayúscula y un número.';
        } else if (backendError?.includes('cantidad de equipos no es válida')) {
          errorMessage = 'La cantidad de equipos seleccionada no es válida. Debe seleccionar entre 4, 6, 8, 10, 12, 14, 16, 18 o 20 equipos.';
        } else if (backendError?.includes('No hay una temporada actual activa')) {
          errorMessage = 'No se puede crear la liga porque no hay una temporada activa. Contacte al administrador del sistema.';
        } else if (backendError?.includes('El alias del equipo ya existe')) {
          errorMessage = 'El nombre del equipo que eligió genera un alias que ya está en uso. Por favor, elija un nombre diferente para su equipo.';
        } else if (backendError?.includes('nombre de equipo ya existe')) {
          errorMessage = 'El nombre del equipo ya está en uso en esta liga. Por favor, elija un nombre diferente para su equipo.';
        } else {
          errorMessage = `Error de validación: ${backendError}`;
        }
      } else if (typeof error.error === 'string') {
        errorMessage = `Error de validación: ${error.error}`;
      } else {
        errorMessage = 'Los datos ingresados no son válidos. Por favor, revise todos los campos y corrija los errores.';
      }
    } else if (error.status === 401) {
      errorMessage = 'Su sesión ha expirado. Por favor, inicie sesión nuevamente para crear la liga.';
    } else if (error.status === 403) {
      errorMessage = 'No tiene permisos para crear una liga. Contacte al administrador del sistema.';
    } else if (error.status === 500) {
      errorMessage = 'Error interno del servidor. Por favor, intente nuevamente en unos minutos. Si el problema persiste, contacte al soporte técnico.';
    } else if (error.status === 0 || !navigator.onLine) {
      errorMessage = 'No se pudo conectar con el servidor. Verifique su conexión a internet e intente nuevamente.';
    } else {
      errorMessage = 'Ocurrió un error inesperado al crear la liga. Por favor, intente nuevamente. Si el problema persiste, contacte al soporte técnico.';
    }
    
    alert(errorMessage);
  }

  /**
   * Marca todos los campos del formulario como tocados para mostrar errores
   */
  private markFormGroupTouched(): void {
    Object.keys(this.leagueForm.controls).forEach(key => {
      this.leagueForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Verifica si un campo específico tiene errores y ha sido tocado
   * @param fieldName - Nombre del campo a validar
   * @returns true si el campo tiene errores, false en caso contrario
   */
  hasFieldError(fieldName: string): boolean {
    const field = this.leagueForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  /**
   * Verifica si hay error de coincidencia de contraseñas
   * @returns true si las contraseñas no coinciden
   */
  hasPasswordMismatch(): boolean {
    return !!(this.leagueForm.errors?.['passwordMismatch'] && 
             this.leagueForm.get('confirmPassword')?.touched);
  }

  /**
   * Obtiene el mensaje de error específico para un campo
   * @param fieldName - Nombre del campo
   * @returns Mensaje de error correspondiente
   */
  getFieldErrorMessage(fieldName: string): string {
    const field = this.leagueForm.get(fieldName);
    
    if (field?.errors?.['required']) {
      return this.getRequiredErrorMessage(fieldName);
    }
    
    if (field?.errors?.['minlength']) {
      return this.getMinLengthErrorMessage(fieldName, field.errors['minlength']?.requiredLength);
    }
    
    if (field?.errors?.['maxlength']) {
      return this.getMaxLengthErrorMessage(fieldName, field.errors['maxlength']?.requiredLength);
    }
    
    if (field?.errors?.['pattern'] && fieldName === 'password') {
      return 'La contraseña debe tener entre 8 y 12 caracteres, ser alfanumérica y contener al menos una letra minúscula, una mayúscula y un número';
    }
    
    return 'Por favor, verifique que el campo esté completado correctamente';
  }

  /**
   * Obtiene mensaje de error específico para campos requeridos
   */
  private getRequiredErrorMessage(fieldName: string): string {
    const messages: { [key: string]: string } = {
      'name': 'El nombre de la liga es obligatorio. Ingrese un nombre único y descriptivo para su liga.',
      'password': 'La contraseña es obligatoria. Debe crear una contraseña segura para proteger su liga.',
      'confirmPassword': 'Debe confirmar la contraseña. Ingrese nuevamente la contraseña para verificar que sea correcta.',
      'commissionerTeamName': 'El nombre de su equipo es obligatorio. Como comisionado, debe asignar un nombre a su equipo.',
      'maxTeams': 'Debe seleccionar la cantidad de equipos que participarán en la liga.',
      'playoffType': 'Debe seleccionar el formato de playoffs para su liga.'
    };
    
    return messages[fieldName] || `El campo ${this.getFieldDisplayName(fieldName)} es obligatorio.`;
  }

  /**
   * Obtiene mensaje de error específico para longitud mínima
   */
  private getMinLengthErrorMessage(fieldName: string, requiredLength: number): string {
    const messages: { [key: string]: string } = {
      'name': `El nombre de la liga debe tener al menos ${requiredLength} caracter. Ingrese un nombre más descriptivo.`,
      'password': `La contraseña debe tener al menos ${requiredLength} caracteres para garantizar la seguridad de su liga.`,
      'commissionerTeamName': `El nombre del equipo debe tener al menos ${requiredLength} caracter. Ingrese un nombre más descriptivo para su equipo.`
    };
    
    return messages[fieldName] || `${this.getFieldDisplayName(fieldName)} debe tener al menos ${requiredLength} caracteres.`;
  }

  /**
   * Obtiene mensaje de error específico para longitud máxima
   */
  private getMaxLengthErrorMessage(fieldName: string, maxLength: number): string {
    const messages: { [key: string]: string } = {
      'name': `El nombre de la liga no puede exceder ${maxLength} caracteres. Ingrese un nombre más conciso.`,
      'description': `La descripción no puede exceder ${maxLength} caracteres. Resuma la información más importante de su liga.`,
      'password': `La contraseña no puede exceder ${maxLength} caracteres. Ingrese una contraseña más corta pero segura.`,
      'commissionerTeamName': `El nombre del equipo no puede exceder ${maxLength} caracteres. Ingrese un nombre más corto para su equipo.`
    };
    
    return messages[fieldName] || `${this.getFieldDisplayName(fieldName)} no puede exceder ${maxLength} caracteres.`;
  }

  /**
   * Obtiene el nombre de display amigable para un campo
   * @param fieldName - Nombre técnico del campo
   * @returns Nombre amigable para mostrar
   */
  private getFieldDisplayName(fieldName: string): string {
    const displayNames: { [key: string]: string } = {
      'name': 'Nombre de la liga',
      'description': 'Descripción',
      'maxTeams': 'Cantidad de equipos',
      'password': 'Contraseña',
      'confirmPassword': 'Confirmación de contraseña',
      'playoffType': 'Tipo de playoffs',
      'commissionerTeamName': 'Nombre del equipo'
    };
    
    return displayNames[fieldName] || fieldName;
  }

  /**
   * Configura la validación en tiempo real del nombre con debounce
   */
  private setupNameValidation(): void {
    this.leagueForm.get('name')?.valueChanges
      .pipe(
        debounceTime(500), // Espera 500ms después del último cambio
        distinctUntilChanged() // Solo procesa si el valor cambió
      )
      .subscribe(name => {
        if (name && name.length >= 3) {
          this.checkNameAvailability(name);
        } else {
          this.nameCheckMessage = '';
          this.nameAvailable = true;
        }
      });
  }

  /**
   * Verifica la disponibilidad del nombre en tiempo real
   * @param name - Nombre a verificar
   */
  private checkNameAvailability(name: string): void {
    this.isCheckingName = true;
    this.nameCheckMessage = 'Verificando disponibilidad...';

    this.leagueService.checkNameAvailability(name).subscribe({
      next: (response) => {
        this.nameAvailable = response.isAvailable;
        this.nameCheckMessage = response.message;
        this.isCheckingName = false;
      },
      error: (error) => {
        console.error('Error verificando nombre:', error);
        this.nameCheckMessage = 'Error verificando disponibilidad';
        this.nameAvailable = true; // Asumimos disponible en caso de error
        this.isCheckingName = false;
      }
    });
  }

  /**
   * Genera un alias automático basado en el nombre del equipo
   * @param teamName - Nombre del equipo
   * @returns Alias generado automáticamente
   */
  private generateAlias(teamName: string): string {
    if (!teamName) return '';
    
    // Tomar las primeras 3 letras de cada palabra significativa
    const words = teamName.split(' ').filter(word => word.length > 2);
    if (words.length === 0) {
      return teamName.substring(0, 3).toUpperCase();
    }
    
    if (words.length === 1) {
      return words[0].substring(0, 3).toUpperCase();
    }
    
    // Si hay múltiples palabras, tomar primera letra de cada una
    return words.map(word => word.charAt(0)).join('').toUpperCase().substring(0, 3);
  }

  /**
   * Actualiza la vista previa del alias cuando cambia el nombre del equipo
   */
  updateAliasPreview(): void {
    const teamName = this.leagueForm.get('commissionerTeamName')?.value || '';
    this.aliasPreview = this.generateAlias(teamName);
  }

  /**
   * Obtiene la vista previa del alias generado
   * @returns Alias generado basado en el nombre actual
   */
  getAliasPreview(): string {
    return this.aliasPreview;
  }

  /**
   * Obtiene información sobre los esquemas por defecto
   * @returns Información de configuración por defecto
   */
  getDefaultSettingsInfo(): string {
    return 'Se aplicarán automáticamente las configuraciones por defecto: posiciones estándar (1 QB, 2 RB, 2 WR, 1 TE, 1 FLEX, 1 K, 1 DEF, 6 BN, 3 IR) y sistema de puntuación estándar';
  }
}