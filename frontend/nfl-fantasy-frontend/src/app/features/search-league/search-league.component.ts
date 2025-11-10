import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LeagueService } from '../../core/services/league.service';
import { League } from '../../shared/models/league.model';

@Component({
  selector: 'app-search-league',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-league.component.html',
  styleUrls: ['./search-league.component.scss'],
})
export class SearchLeagueComponent {
  leagues: League[] = [];
  searchName: string = '';
  showOnlyActive: boolean = false;
  hasSearched: boolean = false;
  loading: boolean = false;

  joinForm = {
    leagueId: 0,
    password: '',
    alias: '',
    teamName: '',
  };

  constructor(private leagueService: LeagueService) {}

  /** Buscar ligas según los filtros */
  searchLeagues() {
    // Validar que al menos haya un filtro
    if (!this.searchName.trim() && !this.showOnlyActive) {
      alert('Por favor, ingrese un nombre o seleccione "Mostrar solo activas".');
      return;
    }

    this.hasSearched = true;
    this.loading = true;
    const filters: any = {};

    if (this.searchName.trim()) filters.name = this.searchName.trim();
    if (this.showOnlyActive) filters.isActive = true;

    this.leagueService.searchLeagues(filters).subscribe({
      next: (res) => {
        this.leagues = res;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error al buscar ligas:', err);
        this.loading = false;
      },
    });
  }

  /** Seleccionar una liga */
  selectLeague(id: number) {
    this.joinForm = { leagueId: id, password: '', alias: '', teamName: '' };
  }

  /** Cancelar unión */
  cancelJoin() {
    this.joinForm = { leagueId: 0, password: '', alias: '', teamName: '' };
  }

  /** Unirse a la liga */
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
