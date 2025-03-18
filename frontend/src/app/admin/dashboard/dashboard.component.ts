import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AdminHeaderFooterComponent } from '../../header-footer/admin-header-footer/admin-header-footer.component';

interface DashboardData {
  totalCategories: number;
  totalProducts: number;
  totalUsers: number;
  totalOrders: number;
  totalOfflineAmount: number;
  totalOnlineAmount: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  imports: [CommonModule, RouterModule, AdminHeaderFooterComponent]
})
export class DashboardComponent implements OnInit {
  totalCategories: number = 0;
  totalProducts: number = 0;
  totalUsers: number = 0;
  totalOrders: number = 0;
  totalOfflineAmount: number = 0;
  totalOnlineAmount: number = 0;

  private baseUrl = 'https://localhost:44304/api';

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    this.fetchDashboardData();
  }

  navigateTo(page: string): void {
    this.router.navigate([`/${page}`]);
  }

  fetchDashboardData(): void {
    const headers = this.getAuthHeaders();
    this.http.get<DashboardData>(`${this.baseUrl}/Dashboard`, { headers }).subscribe({
      next: (data) => {
        this.totalCategories = data.totalCategories;
        this.totalProducts = data.totalProducts;
        this.totalUsers = data.totalUsers;
        this.totalOrders = data.totalOrders;
       
      },
      error: (error) => {
        console.error('Error fetching dashboard data:', error);
      }
    });
  }

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }
}