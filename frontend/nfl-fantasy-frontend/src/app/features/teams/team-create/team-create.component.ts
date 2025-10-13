import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NflTeamService } from '../../../core/services/nflteam.service';

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
    private router: Router
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
    if (this.form.invalid) return;
    const fd = new FormData();
    fd.append('Name', this.form.value.name ?? '');
    fd.append('City', this.form.value.city ?? '');
    if (this.file) fd.append('Image', this.file, this.file.name);

    this.teamService.createTeam(fd).subscribe({
      next: () => {
        alert('Equipo creado');
        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error('Error del backend:', err);

        if (err.error && typeof err.error === 'object') {
          alert(`❌ ${err.error.error || 'Error creando equipo'}`);
        } else if (typeof err.error === 'string') {
          alert(`❌ ${err.error}`);
        } else {
          alert('❌ Error creando equipo');
        }
      },
    });
  }
}
