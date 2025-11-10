import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { NflTeam } from '../../shared/models/team.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class NflTeamService {
  private api = `${environment.apiUrl}/api/nflteams`;

  constructor(private http: HttpClient) {}

  createTeam(formData: FormData): Observable<NflTeam> {
    return this.http.post<NflTeam>(this.api, formData);
  }

  getTeams(): Observable<NflTeam[]> {
    return this.http.get<NflTeam[]>(this.api);
  }
}
