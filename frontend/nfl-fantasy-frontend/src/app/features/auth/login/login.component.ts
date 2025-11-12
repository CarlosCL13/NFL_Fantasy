import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ErrorHandlerService } from '../../../core/services/error-handler.service';

/**
 * Componente de inicio de sesión
 * Usa ErrorHandlerService y maneja la navegación localmente
 */
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  form: any;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private errorHandler: ErrorHandlerService
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  submit() {
    if (this.form.invalid) {
      // Marcar todos los campos como tocados para mostrar los errores
      this.form.markAllAsTouched();
      
      // Generar mensaje de error específico
      let errorMessage = 'Por favor, corrija los siguientes errores:\n';
      
      if (this.form.get('email')?.hasError('required')) {
        errorMessage += '- El email es requerido\n';
      }
      if (this.form.get('email')?.hasError('email')) {
        errorMessage += '- El email no es válido\n';
      }
      if (this.form.get('password')?.hasError('required')) {
        errorMessage += '- La contraseña es requerida\n';
      }
      
      alert(errorMessage);
      return;
    }
    
    this.auth.login(this.form.value as any).subscribe({
      next: () => {
        alert('¡Bienvenido! Has iniciado sesión correctamente.');
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        const errorMessage = this.errorHandler.handleError(error, 'inicio de sesión');
        alert(errorMessage);
      },
    });
  }
}
