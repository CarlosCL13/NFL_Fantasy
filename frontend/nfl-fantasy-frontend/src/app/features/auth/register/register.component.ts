import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ErrorHandlerService } from '../../../core/services/error-handler.service';
import { PasswordValidators } from '../../../shared/validators/password.validators';

/**
 * Componente de registro de usuarios
 * Refactorizado para usar ErrorHandlerService y validadores reutilizables
 * Cumple con Single Responsibility Principle
 */
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  file?: File;
  form: any;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private errorHandler: ErrorHandlerService
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      alias: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
    }, { validators: PasswordValidators.matchPassword('password', 'confirmPassword') });
  }

  onFileChange(event: any) {
    this.file = event.target.files?.[0];
  }

  submit() {
    if (this.form.invalid) {
      // Marcar todos los campos como tocados para mostrar los errores
      this.form.markAllAsTouched();
      
      // Generar mensaje de error específico
      let errorMessage = 'Por favor, corrija los siguientes errores:\n';
      
      if (this.form.get('name')?.hasError('required')) {
        errorMessage += '- El nombre es requerido\n';
      }
      if (this.form.get('email')?.hasError('required')) {
        errorMessage += '- El email es requerido\n';
      }
      if (this.form.get('email')?.hasError('email')) {
        errorMessage += '- El email no es válido\n';
      }
      if (this.form.get('alias')?.hasError('required')) {
        errorMessage += '- El alias es requerido\n';
      }
      if (this.form.get('password')?.hasError('required')) {
        errorMessage += '- La contraseña es requerida\n';
      }
      if (this.form.get('password')?.hasError('minlength')) {
        errorMessage += '- La contraseña debe tener al menos 6 caracteres\n';
      }
      if (this.form.get('confirmPassword')?.hasError('required')) {
        errorMessage += '- La confirmación de contraseña es requerida\n';
      }
      if (this.form.get('confirmPassword')?.hasError('passwordMismatch')) {
        errorMessage += '- Las contraseñas no coinciden\n';
      }
      
      alert(errorMessage);
      return;
    }

    const fd = new FormData();
    fd.append('Name', this.form.value.name ?? '');
    fd.append('Email', this.form.value.email ?? '');
    fd.append('Alias', this.form.value.alias ?? '');
    fd.append('Password', this.form.value.password ?? '');
    fd.append('ConfirmPassword', this.form.value.confirmPassword ?? '');
    if (this.file) fd.append('ProfileImage', this.file, this.file.name);

    this.auth.register(fd).subscribe({
      next: (res: any) => {
        console.log('Respuesta del backend:', res);
        alert('✅ Registro correcto');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        const errorMessage = this.errorHandler.handleError(err, 'registro');
        alert(errorMessage);
      },
    });
  }
}
