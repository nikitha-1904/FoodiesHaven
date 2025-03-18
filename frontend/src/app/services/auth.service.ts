import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root' // Provided globally
})
export class AuthService {
  private baseUrl = 'https://localhost:44304/api/Auth';

  constructor(private http: HttpClient, private router: Router) {}

  // User Registration
  register(userData: any): Observable<any> {
    console.log(userData);
    return this.http.post(`${this.baseUrl}/signup`, userData, { responseType: 'text' }).pipe(
      tap(response => {
        console.log('Registration Successful:', response);
        this.router.navigate(['/app-user-login']); // Navigate to login page after successful registration
      }),
      catchError(this.handleError)
    );
  }

  // User Login
  login(loginData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, loginData).pipe(
      tap((response: any) => {
        console.log('Login Response:', response); // temp

        if (response.token) {
          localStorage.setItem('token', response.token);
          const decodedToken: any = jwtDecode(response.token);
          console.log('Decoded Token:', decodedToken); // Log the decoded token

          localStorage.setItem('role', decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']);
          localStorage.setItem('username', decodedToken.sub);
          localStorage.setItem('id', decodedToken.jti);
          console.log('Role Stored:', decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']); // temp
        } else {
          console.error('Role not found'); // temp
        }
      }),
      catchError(this.handleError)
    );
  }

  // Error Handling
  private handleError(error: any): Observable<never> {
    console.error('An error occurred:', error);
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getUserRole(): string | null {
    return localStorage.getItem('role');
  }

  logout(): void {
    localStorage.clear();
  }
}
