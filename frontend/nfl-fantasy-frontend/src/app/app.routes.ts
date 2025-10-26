import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { TeamCreateComponent } from './features/teams/team-create/team-create.component';
import { SeasonCreateComponent } from './features/season/season-create/season-create.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'teams/create', component: TeamCreateComponent },
  { path: 'seasons/create', component: SeasonCreateComponent },
];
