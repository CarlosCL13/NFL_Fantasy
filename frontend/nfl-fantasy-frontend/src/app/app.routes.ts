import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { TeamCreateComponent } from './features/teams/team-create/team-create.component';
import { SeasonCreateComponent } from './features/season/season-create/season-create.component';
import { SearchLeagueComponent } from './features/search-league/search-league.component';
import { LeagueCreateComponent } from './features/league/league-create/league-create.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'leagues/search', component: SearchLeagueComponent }, //
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'teams/create', component: TeamCreateComponent, canActivate: [AuthGuard] },
  { path: 'seasons/create', component: SeasonCreateComponent, canActivate: [AuthGuard] },
  { path: 'leagues/create', component: LeagueCreateComponent, canActivate: [AuthGuard] },
];
