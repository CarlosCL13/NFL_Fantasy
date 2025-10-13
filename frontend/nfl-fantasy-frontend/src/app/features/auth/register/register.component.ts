import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  file?: File;

  form: any; // declaras sin inicializar

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {}

  ngOnInit() {
    // ahora sí: fb ya está inicializado
    this.form = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      alias: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
    });
  }

  onFileChange(event: any) {
    this.file = event.target.files?.[0];
  }

  submit() {
    if (this.form.invalid) return;
    if (this.form.value.password !== this.form.value.confirmPassword) {
      alert('Las contraseñas no coinciden');
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
        console.error('Error del backend:', err);

        if (err.error && typeof err.error === 'object') {
          alert(`❌ ${err.error.error || 'Error en el registro'}`);
        } else if (typeof err.error === 'string') {
          alert(`❌ ${err.error}`);
        } else {
          alert('❌ Error en el registro');
        }
      },
    });
  }
}
