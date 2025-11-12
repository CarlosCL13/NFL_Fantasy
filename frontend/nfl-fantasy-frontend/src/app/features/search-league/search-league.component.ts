import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LeagueService } from '../../core/services/league.service';
import { League } from '../../shared/models/league.model';
import { ErrorHandlerService } from '../../core/services/error-handler.service';

/**
 * Componente para buscar y unirse a ligas
 */
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

  constructor(
    private leagueService: LeagueService,
    private errorHandler: ErrorHandlerService
  ) {}

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
        const errorMessage = this.errorHandler.handleError(err, 'búsqueda de ligas');
        alert(errorMessage);
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

    // Validar campos requeridos
    let errorMessage = '';
    
    if (!this.joinForm.password.trim()) {
      errorMessage += '- La contraseña de la liga es requerida\n';
    }
    if (!this.joinForm.alias.trim()) {
      errorMessage += '- El alias de su equipo es requerido\n';
    }
    if (!this.joinForm.teamName.trim()) {
      errorMessage += '- El nombre de su equipo es requerido\n';
    }
    
    if (errorMessage) {
      alert('Por favor, corrija los siguientes errores:\n' + errorMessage);
      return;
    }

    this.leagueService.joinLeague(this.joinForm).subscribe({
      next: (res) => {
        alert((res?.message || 'Te uniste correctamente a la liga.'));
        this.cancelJoin();
      },
      error: (err) => {
        const errorMessage = this.errorHandler.handleError(err, 'unirse a la liga');
        alert(errorMessage);
      },
    });
  }
}
