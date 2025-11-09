import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SeasonService } from '../../core/services/season.service';
import { LeagueService } from '../../core/services/league.service';
import { Season } from '../../shared/models/season.model';
import { League } from '../../shared/models/league.model';

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

  loadSeasons() {
    this.seasonService.getAllSeasons().subscribe({
      next: (res) => (this.seasons = res),
      error: (err) => console.error('Error al cargar seasons:', err),
    });
  }

  onSelectSeason() {
    if (!this.selectedSeasonId) return;
    const filters = { seasonId: this.selectedSeasonId.toString() };

    this.leagueService.searchLeagues(filters).subscribe({
      next: (res) => (this.leagues = res),
      error: (err) => console.error('Error al cargar ligas:', err),
    });
  }

  /** Cuando el usuario selecciona una liga */
  selectLeague(id: number) {
    this.joinForm.leagueId = id;
    this.joinForm.password = '';
    this.joinForm.alias = '';
    this.joinForm.teamName = '';
  }

  /** Permite cancelar la unión */
  cancelJoin() {
    this.joinForm = { leagueId: 0, password: '', alias: '', teamName: '' };
  }

  /** Envía la solicitud para unirse */
  joinLeague() {
    if (!this.joinForm.leagueId) {
      alert('Seleccione una liga para unirse');
      return;
    }

    this.leagueService.joinLeague(this.joinForm).subscribe({
      next: (res) => {
        alert(res?.message || 'Te uniste correctamente a la liga.');
        this.cancelJoin();
      },
      error: (err) => {
        console.error('Error al unirse a la liga:', err);
        if (err.error) {
          alert(err.error.error || 'Error desconocido al unirse a la liga.');
        } else {
          alert('Ocurrió un error inesperado.');
        }
      },
    });
  }
}
