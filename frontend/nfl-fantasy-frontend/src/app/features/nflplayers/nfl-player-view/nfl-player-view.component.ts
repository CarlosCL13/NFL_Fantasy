import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { NflPlayerService } from '../../../core/services/nflplayer.service';
import { ErrorHandlerService } from '../../../core/services/error-handler.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-nfl-player-view',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './nfl-player-view.component.html',
  styleUrls: ['./nfl-player-view.component.scss'],
})
export class NflPlayerViewComponent implements OnInit {
  players: any[] = [];
  selectedPlayer: any = null;
  playerNews: any[] = [];
  designaciones: any[] = [];

  loadingNews = false;
  showModal = false;
  errorMessage = '';
  submitting = false;

  noticia = {
    texto: '',
    resumen: '',
    isLesion: false,
    designacionId: null,
  };

  constructor(
    private playerService: NflPlayerService,
    private errorHandler: ErrorHandlerService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPlayers();
    this.loadDesignaciones();
  }

  loadPlayers() {
    this.playerService.getPlayers().subscribe({
      next: (data) => (this.players = data),
      error: (err) => alert(this.errorHandler.handleError(err, 'cargar jugadores')),
    });
  }

  loadDesignaciones() {
    this.playerService.getDesignaciones().subscribe({
      next: (data) => (this.designaciones = data),
      error: (err) => alert(this.errorHandler.handleError(err, 'cargar designaciones')),
    });
  }

  selectPlayer(player: any) {
    this.selectedPlayer = player;
    this.showModal = true;
    this.noticia = { texto: '', resumen: '', isLesion: false, designacionId: null };
    this.loadNews(player.nflPlayerId);
  }

  loadNews(playerId: number) {
    this.loadingNews = true;

    this.playerService.getPlayerNews(playerId).subscribe({
      next: (data) => {
        this.playerNews = data.sort(
          (a: any, b: any) =>
            new Date(b.fechaCreacion).getTime() - new Date(a.fechaCreacion).getTime()
        );
        this.loadingNews = false;
      },
      error: (err) => {
        this.loadingNews = false;
        alert(this.errorHandler.handleError(err, 'cargar noticias'));
      },
    });
  }

  validateNews() {
    const t = this.noticia.texto.trim();
    if (t.length < 10 || t.length > 300) {
      return 'El texto debe tener entre 10 y 300 caracteres.';
    }

    if (this.noticia.isLesion) {
      if (!this.noticia.resumen || this.noticia.resumen.length > 30) {
        return 'El resumen es obligatorio y no puede superar 30 caracteres.';
      }
      if (!this.noticia.designacionId) {
        return 'Debe seleccionar una designación.';
      }
    }

    return null;
  }

  submitNews() {
    this.errorMessage = '';

    const validation = this.validateNews();
    if (validation) {
      this.errorMessage = validation;
      return;
    }

    const body = {
      playerId: this.selectedPlayer.nflPlayerId,
      texto: this.noticia.texto,
      resumen: this.noticia.resumen,
      isLesion: this.noticia.isLesion,
      designacionId: this.noticia.isLesion ? this.noticia.designacionId : null,
    };

    this.submitting = true;

    this.playerService.createPlayerNews(body).subscribe({
      next: () => {
        this.submitting = false;
        this.loadNews(this.selectedPlayer.nflPlayerId);
        alert('Noticia agregada correctamente.');
      },
      error: (err) => {
        this.submitting = false;
        this.errorMessage = this.errorHandler.handleError(err, 'crear noticia');
      },
    });
  }

  goToCreatePlayer() {
    this.router.navigate(['/players/create']);
  }

  closeModal() {
    this.showModal = false;
    this.selectedPlayer = null;
  }

  isAdmin() {
    return this.authService.getUserRole() === 'admin';
  }
}
