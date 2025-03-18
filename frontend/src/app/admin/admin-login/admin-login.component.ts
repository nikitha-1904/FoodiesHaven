import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-admin-login',
  imports: [FormsModule],
  templateUrl: './admin-login.component.html',
  styleUrls: ['./admin-login.component.css']
})
export class AdminLoginComponent {
  username: string = '';
  password: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  onLogin(): void {
    const loginData = { username: this.username, password: this.password };
    console.log(loginData);
    this.authService.login(loginData).subscribe(
      response => {
        console.log('Login successful', response);
        const token = response.token;
        const decodedToken: any = jwtDecode(token);
        console.log('Decoded Token:', decodedToken); // Log the decoded token

        const userRole = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        console.log('User Role:', userRole); // Log the user role

        if (userRole === 'admin') {
          // Navigate to admin menu if the user is an admin
          this.router.navigate(['/app-admin-menu']);
        } else {
          // Show error message if the user is not an admin
          console.error('Permissions not granted. Only admins can log in.');
          alert('Permissions not granted. Only admins can log in.');
        }
      },
      error => {
        console.error('Login failed', error);
        // Show error message
        alert('Login failed. Please check your credentials and try again.');
      }
    );
  }
}