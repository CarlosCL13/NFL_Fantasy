import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SeasonService } from '../../core/services/season.service';
import { LeagueService } from '../../core/services/league.service';
import { Season } from '../../shared/models/season.model';
import { League, CreateLeagueResponse } from '../../shared/models/league.model';

@Component({
  selector: 'app-search-league',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-league.component.html',
  styleUrls: ['./search-league.component.scss'],
})
export class SearchLeagueComponent implements OnInit {
  seasons: Season[] = [];
  leagues: League[] = [];
  selectedSeasonId: number | null = null;

  joinForm = {
    leagueId: 0,
    password: '',
    alias: '',
    teamName: '',
  };

  constructor(private seasonService: SeasonService, private leagueService: LeagueService) {}

  ngOnInit() {
    this.loadSeasons();
  }

  /** Carga todas las temporadas desde SeasonService */
  loadSeasons() {
    this.seasonService.getAllSeasons().subscribe({
      next: (res) => (this.seasons = res),
      error: (err) => console.error('Error al cargar seasons:', err),
    });
  }

  /** Cuando el usuario selecciona una temporada, carga ligas correspondientes */
  onSelectSeason() {
    if (!this.selectedSeasonId) return;

    const filters = { seasonId: this.selectedSeasonId.toString() };

    this.leagueService.searchLeagues(filters).subscribe({
      next: (res) => (this.leagues = res),
      error: (err) => console.error('Error al cargar ligas:', err),
    });
  }

  /** Permite unirse a la liga seleccionada */
  joinLeague() {
    if (!this.joinForm.leagueId) {
      alert('Seleccione una liga para unirse');
      return;
    }

    this.leagueService.createLeague(this.joinForm as any).subscribe({
      next: (res: CreateLeagueResponse) => {
        alert('Te uniste a la liga correctamente');
      },
      error: (err) => {
        console.error('Error al unirse:', err);
        alert('Error al unirse a la liga (revisa tu token o credenciales)');
      },
    });
  }

  /** Verifica disponibilidad de nombre de liga */
  checkLeagueName(name: string) {
    this.leagueService.checkNameAvailability(name).subscribe({
      next: (res) => {
        if (!res.isAvailable) {
          alert(res.message);
        }
      },
      error: (err) => console.error('Error al verificar nombre de liga:', err),
    });
  }
}
