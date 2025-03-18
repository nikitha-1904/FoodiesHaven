import { Component, OnInit } from '@angular/core';
import { AdminHeaderFooterComponent } from '../../header-footer/admin-header-footer/admin-header-footer.component';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';

interface User {
  userId: number;
  name: string;
  username: string;
  mobile: string;
  email: string;
  address: string;
  role: string;
  createdDate: string;
}

@Component({
  selector: 'app-users-admin-info',
  imports: [AdminHeaderFooterComponent, CommonModule],
  templateUrl: './users-admin-info.component.html',
  styleUrls: ['./users-admin-info.component.css']
})
export class UsersAdminInfoComponent implements OnInit {
  users: User[] = [];
  filteredUsers: User[] = [];
  baseUrl: string = 'https://localhost:44304/api/User';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.fetchUsers();
  }

  fetchUsers(): void {
    const headers = this.getAuthHeaders();
    this.http.get<User[]>(this.baseUrl, { headers }).subscribe({
      next: (data) => {
        this.users = data;
        this.filteredUsers = data;
      },
      error: (error) => {
        console.error('Error fetching users:', error);
      }
    });
  }

  filterUsersByRole(event: Event): void {
    const selectedRole = (event.target as HTMLSelectElement).value;
    if (selectedRole) {
      this.filteredUsers = this.users.filter(user =>
        user.role.toLowerCase() === selectedRole.toLowerCase()
      );
    } else {
      this.filteredUsers = this.users;
    }
  }

  filterUsersByUsername(event: Event): void {
    const searchTerm = (event.target as HTMLInputElement).value.toLowerCase();
    this.filteredUsers = this.users.filter(user =>
      user.username.toLowerCase().includes(searchTerm)
    );
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }
}