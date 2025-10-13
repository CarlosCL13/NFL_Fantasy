import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { LoginDto } from '../../shared/models/login.dto';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private api = `${environment.apiUrl}/api/users`;

  constructor(private http: HttpClient, private router: Router) {}

  register(formData: FormData) {
    // formData: multipart/form-data
    return this.http.post(`${this.api}/register`, formData);
  }

  login(dto: LoginDto): Observable<any> {
    return this.http.post(`${this.api}/login`, dto).pipe(
      tap((res: any) => {
        // suponemos que res.token contiene el JWT
        if (res?.token) {
          localStorage.setItem('token', res.token);
          // opcional: guardar user
          localStorage.setItem('user', JSON.stringify(res.user || {}));
        }
      })
    );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
