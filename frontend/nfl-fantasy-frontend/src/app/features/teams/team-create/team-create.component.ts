import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NflTeamService } from '../../../core/services/nflteam.service';
import { ErrorHandlerService } from '../../../core/services/error-handler.service';

/**
 * Componente para crear equipos NFL
 * Refactorizado para usar ErrorHandlerService
 * Cumple con Single Responsibility Principle
 */
@Component({
  selector: 'app-team-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './team-create.component.html',
  styleUrl: './team-create.component.scss',
})
export class TeamCreateComponent {
  file?: File;
  form: any;

  constructor(
    private fb: FormBuilder,
    private teamService: NflTeamService,
    private router: Router,
    private errorHandler: ErrorHandlerService
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      city: ['', Validators.required],
    });
  }

  onFileChange(event: any) {
    this.file = event.target.files?.[0];
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      
      let errorMessage = 'Por favor, corrija los siguientes errores:\n';
      
      if (this.form.get('name')?.hasError('required')) {
        errorMessage += '- El nombre del equipo es requerido\n';
      }
      if (this.form.get('city')?.hasError('required')) {
        errorMessage += '- La ciudad del equipo es requerida\n';
      }
      
      alert(errorMessage);
      return;
    }

    const fd = new FormData();
    fd.append('Name', this.form.value.name ?? '');
    fd.append('City', this.form.value.city ?? '');
    if (this.file) fd.append('Image', this.file, this.file.name);

    this.teamService.createTeam(fd).subscribe({
      next: () => {
        alert('Equipo creado exitosamente');
        this.router.navigate(['/']);
      },
      error: (err) => {
        const errorMessage = this.errorHandler.handleError(err, 'creación de equipo');
        alert(errorMessage);
      },
    });
  }
}
