import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LeagueService } from '../../core/services/league.service';

@Component({
  selector: 'app-search-league',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-league.component.html',
  styleUrl: './search-league.component.scss',
})
export class SearchLeagueComponent implements OnInit {
  seasons: any[] = [];
  leagues: any[] = [];
  selectedSeasonId: number | null = null;

  joinForm = {
    leagueId: 0,
    password: '',
    alias: '',
    teamName: '',
  };

  constructor(private leagueService: LeagueService) {}

  ngOnInit() {
    this.loadSeasons();
  }

  loadSeasons() {
    this.leagueService.getSeasons().subscribe({
      next: (res) => (this.seasons = res),
      error: (err) => console.error('Error al cargar seasons:', err),
    });
  }

  onSelectSeason() {
    if (!this.selectedSeasonId) return;
    this.leagueService.searchLeagues(this.selectedSeasonId).subscribe({
      next: (res) => (this.leagues = res),
      error: (err) => console.error('Error al cargar ligas:', err),
    });
  }

  joinLeague() {
    if (!this.joinForm.leagueId) {
      alert('Seleccione una liga para unirse');
      return;
    }

    this.leagueService.joinLeague(this.joinForm).subscribe({
      next: () => {
        alert('Te uniste a la liga correctamente');
      },
      error: (err) => {
        console.error('Error al unirse:', err);
        alert('Error al unirse a la liga (revisa tu token o credenciales)');
      },
    });
  }
}
