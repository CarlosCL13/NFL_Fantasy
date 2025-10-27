import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard-container">
      <div class="header">
        <h1>NFL Fantasy Dashboard</h1>
        <div class="user-actions">
          <span>¡Bienvenido!</span>
          <button class="btn btn-secondary" (click)="logout()">Cerrar Sesión</button>
        </div>
      </div>
      
      <div class="dashboard-grid">
        <div class="card action-card" (click)="navigateTo('/leagues/create')">
          <div class="card-icon">🏆</div>
          <h3>Crear Liga</h3>
          <p>Crea una nueva liga de fantasy y conviértete en comisionado</p>
        </div>
        
        <div class="card action-card" (click)="navigateTo('/teams/create')">
          <div class="card-icon">👥</div>
          <h3>Crear Equipo</h3>
          <p>Crea un nuevo equipo para unirte a una liga existente</p>
        </div>
        
        <div class="card action-card" (click)="navigateTo('/seasons/create')">
          <div class="card-icon">📅</div>
          <h3>Crear Temporada</h3>
          <p>Administra las temporadas del sistema (Solo administradores)</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 2rem;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      color: white;
      margin-bottom: 3rem;
    }

    .header h1 {
      margin: 0;
      font-size: 2.5rem;
      font-weight: 600;
    }

    .user-actions {
      display: flex;
      align-items: center;
      gap: 1rem;
    }

    .dashboard-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 2rem;
      max-width: 1200px;
      margin: 0 auto;
    }

    .card {
      background: white;
      border-radius: 12px;
      padding: 2rem;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }

    .action-card {
      cursor: pointer;
      transition: all 0.3s ease;
      text-align: center;
    }

    .action-card:hover {
      transform: translateY(-5px);
      box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
    }

    .card-icon {
      font-size: 3rem;
      margin-bottom: 1rem;
    }

    .card h3 {
      margin: 0 0 1rem 0;
      color: #374151;
      font-size: 1.5rem;
    }

    .card p {
      margin: 0;
      color: #6b7280;
      line-height: 1.5;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 8px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s ease;
    }

    .btn-secondary {
      background: rgba(255, 255, 255, 0.2);
      color: white;
      border: 2px solid rgba(255, 255, 255, 0.3);
    }

    .btn-secondary:hover {
      background: rgba(255, 255, 255, 0.3);
    }

    @media (max-width: 768px) {
      .dashboard-container {
        padding: 1rem;
      }

      .header {
        flex-direction: column;
        gap: 1rem;
        text-align: center;
      }

      .header h1 {
        font-size: 2rem;
      }

      .dashboard-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class DashboardComponent {
  constructor(
    private router: Router,
    private authService: AuthService
  ) {}

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }

  logout(): void {
    this.authService.logout();
  }
}