import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-add-category',
  imports: [CommonModule, FormsModule],
  templateUrl: './add-category.component.html',
  styleUrls: ['./add-category.component.css']
})
export class AddCategoryComponent {
  category = {
    name: '',
    isActive: false
  };

  private baseUrl = 'https://localhost:44304/api/Category';

  constructor(private http: HttpClient, private router: Router) {}

  saveCategory(): void {
    const headers = this.getAuthHeaders();
    const requestBody = { name: this.category.name, isActive: this.category.isActive };

    this.http.post(`${this.baseUrl}`, requestBody, { headers }).subscribe({
      next: () => {
        alert('Category added successfully!');
        this.router.navigate(['/app-admin-menu']); // Navigate to admin menu or any other route after adding category
      },
      error: (error) => {
        console.error('Error adding category:', error);
        alert('Failed to add category.');
      }
    });
  }

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }
}
