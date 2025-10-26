import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { SeasonService } from '../../../core/services/season.service';
import { CreateSeasonDto } from '../../../shared/models/season.model';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-season-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './season-create.component.html',
  styleUrl: './season-create.component.scss'
})
export class SeasonCreateComponent {
  seasonForm: FormGroup;
  isSubmitting = false;
  isCheckingName = false;
  nameAvailable = true;
  nameCheckMessage = '';
  currentSeasonInfo: any = null;

  constructor(
    private formBuilder: FormBuilder,
    private seasonService: SeasonService,
    public router: Router
  ) {
    this.seasonForm = this.createSeasonForm();
    this.setupNameValidation();
    this.loadCurrentSeasonInfo();
  }

  /**
   * Crea el formulario reactivo para la temporada con validaciones
   * @returns FormGroup configurado para crear temporada
   */
  private createSeasonForm(): FormGroup {
    return this.formBuilder.group({
      name: ['', [
        Validators.required, 
        Validators.minLength(1), 
        Validators.maxLength(100)
      ]],
      weeksCount: [17, [
        Validators.required, 
        Validators.min(1), 
        Validators.max(30)
      ]],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      isCurrent: [false]
    });
  }

  /**
   * Valida que la fecha de fin sea posterior a la fecha de inicio
   * @returns true si las fechas son válidas, false en caso contrario
   */
  private validateDates(): boolean {
    const fechaInicio = new Date(this.seasonForm.get('startDate')?.value);
    const fechaFin = new Date(this.seasonForm.get('endDate')?.value);
    
    return fechaFin > fechaInicio;
  }

  /**
   * Maneja el envío del formulario para crear la temporada
   */
  onSubmit(): void {
    if (this.seasonForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    if (!this.validateDates()) {
      alert('❌ La fecha de fin debe ser posterior a la fecha de inicio');
      return;
    }

    this.isSubmitting = true;
    
    const createSeasonDto: CreateSeasonDto = {
      name: this.seasonForm.get('name')?.value,
      weeksCount: this.seasonForm.get('weeksCount')?.value,
      startDate: new Date(this.seasonForm.get('startDate')?.value),
      endDate: new Date(this.seasonForm.get('endDate')?.value),
      isCurrent: this.seasonForm.get('isCurrent')?.value
    };

    this.seasonService.createSeason(createSeasonDto).subscribe({
      next: (response) => {
        alert('✅ Temporada creada exitosamente');
        this.router.navigate(['/']);
      },
      error: (error) => {
        console.error('Error al crear temporada:', error);
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
    if (error.error && typeof error.error === 'object') {
      alert(`❌ ${error.error.error || 'Error al crear la temporada'}`);
    } else if (typeof error.error === 'string') {
      alert(`❌ ${error.error}`);
    } else {
      alert('❌ Error al crear la temporada. Por favor, intente nuevamente.');
    }
  }

  /**
   * Marca todos los campos del formulario como tocados para mostrar errores
   */
  private markFormGroupTouched(): void {
    Object.keys(this.seasonForm.controls).forEach(key => {
      this.seasonForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Verifica si un campo específico tiene errores y ha sido tocado
   * @param fieldName - Nombre del campo a validar
   * @returns true si el campo tiene errores, false en caso contrario
   */
  hasFieldError(fieldName: string): boolean {
    const field = this.seasonForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  /**
   * Obtiene el mensaje de error específico para un campo
   * @param fieldName - Nombre del campo
   * @returns Mensaje de error correspondiente
   */
  getFieldErrorMessage(fieldName: string): string {
    const field = this.seasonForm.get(fieldName);
    
    if (field?.errors?.['required']) {
      return `El campo ${fieldName} es obligatorio`;
    }
    
    if (field?.errors?.['minlength'] || field?.errors?.['min']) {
      return `El ${fieldName} debe tener al menos ${field.errors['minlength']?.requiredLength || field.errors['min']?.min} caracteres/valor`;
    }
    
    if (field?.errors?.['maxlength'] || field?.errors?.['max']) {
      return `El ${fieldName} no debe exceder ${field.errors['maxlength']?.requiredLength || field.errors['max']?.max} caracteres/valor`;
    }
    
    return 'Campo inválido';
  }

  /**
   * Configura la validación en tiempo real del nombre con debounce
   */
  private setupNameValidation(): void {
    this.seasonForm.get('name')?.valueChanges
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

    this.seasonService.checkNameAvailability(name).subscribe({
      next: (response) => {
        this.nameAvailable = response.isAvailable;
        this.nameCheckMessage = response.message;
        this.isCheckingName = false;
      },
      error: (error) => {
        console.error('Error verificando nombre:', error);
        this.nameCheckMessage = 'Error verificando disponibilidad';
        this.isCheckingName = false;
      }
    });
  }

  /**
   * Carga información sobre la temporada actual
   */
  private loadCurrentSeasonInfo(): void {
    this.seasonService.getCurrentSeason().subscribe({
      next: (season) => {
        this.currentSeasonInfo = season;
      },
      error: (error) => {
        // No hay temporada actual, esto es normal
        this.currentSeasonInfo = null;
      }
    });
  }

  /**
   * Verifica si se puede marcar como temporada actual
   * @returns true si se puede marcar como actual
   */
  canSetAsCurrent(): boolean {
    return !this.currentSeasonInfo || !this.seasonForm.get('isCurrent')?.value;
  }

  /**
   * Obtiene mensaje informativo sobre temporada actual
   * @returns Mensaje informativo
   */
  getCurrentSeasonMessage(): string {
    if (!this.currentSeasonInfo) {
      return 'No hay temporada activa actualmente';
    }
    return `Temporada actual: "${this.currentSeasonInfo.name}" (${new Date(this.currentSeasonInfo.startDate).getFullYear()})`;
  }
}