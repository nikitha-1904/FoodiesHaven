import { Component, OnInit } from '@angular/core';
import { UserHeaderFooterComponent } from '../../header-footer/user-header-footer/user-header-footer.component';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { jwtDecode } from 'jwt-decode';
import { Router, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';

interface User {
  name: string;
  username: string;
  mobile: string;
  email: string;
  address: string[];
  pinCode: string;
  password: string;
  createdDate: string;
  role:string;
}

@Component({
  selector: 'app-user-info',
  imports: [UserHeaderFooterComponent, CommonModule, FormsModule],
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css']
})
export class UserInfoComponent implements OnInit {
  user: User | null = null;
  isEditing = false;

  constructor(private http: HttpClient, private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.getUserData();
    });
  }

  updateUser(): void {
    if (this.user) {
      const headers = this.getAuthHeaders();
      const url = `https://localhost:44304/api/User/${this.user.username}`;
      const userDTO = {
        name: this.user.name,
        username: this.user.username,
        mobile: this.user.mobile,
        email: this.user.email,
        address: Array.isArray(this.user.address) ? this.user.address : [this.user.address], // Ensure address is an array
        pinCode: this.user.pinCode,
        password: this.user.password,
        createdDate: this.user.createdDate,
        role: 'user' // Add role and set it to "user"

      };
      this.http.put<User>(url, userDTO, { headers }).subscribe(
        (data) => {
          this.user = data;
          this.isEditing = false;
          console.log('User updated successfully:', data);
          this.router.navigate(['/app-user-info']).then(() => {
            window.location.reload(); // Reload the page to fetch updated user data
          });
        },
        (error) => {
          console.error('Error updating user data:', error);
          if (error.status === 400 && error.error.errors) {
            for (const key in error.error.errors) {
              if (error.error.errors.hasOwnProperty(key)) {
                console.error(`${key}: ${error.error.errors[key]}`);
              }
            }
          }
        }
      );
    }
  }
  toggleEdit(): void {
    this.isEditing = !this.isEditing;
  }

  logout(): void {
    localStorage.clear();
    this.router.navigate(['/app-home']);

  }
  navigateToOrderHistory() {
    this.router.navigate(['/app-order-history']);
  }

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    console.log('Token:', token);
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }

  private fetchUsernameFromToken(): string | null {
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      return decodedToken.sub; // Ensure the correct field is used to get the username
    }
    return null;
  }

  private getUserData() {
    const username = this.fetchUsernameFromToken();
    if (username) {
      const headers = this.getAuthHeaders();
      const url = `https://localhost:44304/api/User/${username}`;
      this.http.get<User>(url, { headers }).subscribe(
        (data) => {
          this.user = data;
        },
        (error) => {
          console.error('Error fetching user data:', error);
        }
      );
    }
  }
}
