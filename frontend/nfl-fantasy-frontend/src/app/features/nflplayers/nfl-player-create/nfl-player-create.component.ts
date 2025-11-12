import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NflPlayerService } from '../../../core/services/nflplayer.service';
import { NflTeamService } from '../../../core/services/nflteam.service';
import { ErrorHandlerService } from '../../../core/services/error-handler.service';

/**
 * Componente para crear jugadores NFL
 * Refactorizado para usar ErrorHandlerService
 */
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
    private router: Router,
    private errorHandler: ErrorHandlerService
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
      error: (err) => {
        const errorMessage = this.errorHandler.handleError(err, 'carga de posiciones');
        alert(errorMessage);
      },
    });
  }

  loadTeams(): void {
    this.teamService.getTeams().subscribe({
      next: (data) => (this.teams = data),
      error: (err) => {
        const errorMessage = this.errorHandler.handleError(err, 'carga de equipos NFL');
        alert(errorMessage);
      },
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
      this.form.markAllAsTouched();
      
      let errorMessage = 'Por favor, corrija los siguientes errores:\n';
      
      if (this.form.get('name')?.hasError('required')) {
        errorMessage += '- El nombre del jugador es requerido\n';
      }
      if (this.form.get('positionId')?.hasError('required')) {
        errorMessage += '- Debe seleccionar una posición\n';
      }
      if (this.form.get('nflTeamId')?.hasError('required')) {
        errorMessage += '- Debe seleccionar un equipo NFL\n';
      }
      if (this.form.get('image')?.hasError('required')) {
        errorMessage += '- Debe seleccionar una imagen del jugador\n';
      }
      
      alert(errorMessage);
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
        const errorMessage = this.errorHandler.handleError(err, 'creación de jugador');
        alert(errorMessage);
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
        // Para bulk upload, mostramos el error específico del JSON
        if (err.error?.errors) {
          this.bulkError = err.error.errors.join('\n');
        } else {
          const errorMessage = this.errorHandler.handleError(err, 'carga masiva de jugadores');
          this.bulkError = errorMessage;
        }
        this.bulkMessage = '';
      },
    });
  }
}
