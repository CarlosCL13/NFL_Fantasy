import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { NflTeam } from '../../shared/models/team.model';

@Injectable({ providedIn: 'root' })
export class NflTeamService {
  private api = `${environment.apiUrl}/api/nflteams`;

  constructor(private http: HttpClient) {}

  createTeam(formData: FormData) {
    return this.http.post(this.api, formData);
  }

  // otros métodos CRUD...
}
