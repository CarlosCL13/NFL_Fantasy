import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NflPlayerService } from '../../../core/services/nflplayer.service';
import { NflTeamService } from '../../../core/services/nflteam.service';

@Component({
  selector: 'app-nfl-player-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './nfl-player-create.component.html',
  styleUrls: ['./nfl-player-create.component.scss'],
})
export class NflPlayerCreateComponent implements OnInit {
  positions: any[] = [];
  teams: any[] = [];
  file?: File;
  form: any;

  bulkFile?: File;
  bulkMessage = '';
  bulkError = '';

  constructor(
    private fb: FormBuilder,
    private playerService: NflPlayerService,
    private teamService: NflTeamService,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      positionId: ['', Validators.required],
      nflTeamId: ['', Validators.required],
      image: [null, Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadPositions();
    this.loadTeams();
  }

  loadPositions(): void {
    this.playerService.getPositions().subscribe({
      next: (data) => (this.positions = data),
      error: () => alert('Error cargando posiciones.'),
    });
  }

  loadTeams(): void {
    this.teamService.getTeams().subscribe({
      next: (data) => (this.teams = data),
      error: () => alert('Error cargando equipos NFL.'),
    });
  }

  onFileChange(event: any): void {
    const file = event.target.files?.[0];
    if (file) {
      this.file = file;
      this.form.patchValue({ image: file });
    }
  }

  submit(): void {
    if (this.form.invalid) {
      alert('⚠️ Debes completar todos los campos obligatorios.');
      return;
    }

    const fd = new FormData();
    fd.append('Name', this.form.value.name ?? '');
    fd.append('PositionId', this.form.value.positionId ?? '');
    fd.append('NflTeamId', this.form.value.nflTeamId ?? '');
    if (this.file) fd.append('Image', this.file, this.file.name);

    this.playerService.createPlayer(fd).subscribe({
      next: (res) => {
        alert('✅ ' + res.message);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.error(err);
        if (err.error?.error) {
          alert('❌ ' + err.error.error);
        } else {
          alert('❌ Error creando jugador.');
        }
      },
    });
  }

  onBulkFileChange(event: any) {
    this.bulkFile = event.target.files?.[0];
    this.bulkMessage = '';
    this.bulkError = '';
  }

  uploadBulk() {
    if (!this.bulkFile) {
      this.bulkError = 'Debes seleccionar un archivo JSON antes de subirlo.';
      return;
    }

    this.playerService.bulkUpload(this.bulkFile).subscribe({
      next: (res: any) => {
        this.bulkMessage = res.message || '✅ Jugadores cargados exitosamente.';
        this.bulkError = '';
      },
      error: (err) => {
        console.error('Error al subir archivo JSON:', err);
        this.bulkError =
          err.error?.errors?.join('\n') ||
          err.error?.error ||
          '❌ Error procesando el archivo JSON.';
        this.bulkMessage = '';
      },
    });
  }
}
