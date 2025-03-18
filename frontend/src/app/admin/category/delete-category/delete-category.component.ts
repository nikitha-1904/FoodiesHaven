import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-delete-category',
  imports: [CommonModule, FormsModule],
  templateUrl: './delete-category.component.html',
  styleUrls: ['./delete-category.component.css']
})
export class DeleteCategoryComponent {
  categoryName: string = '';

  private baseUrl = 'https://localhost:44304/api/Category';

  constructor(private http: HttpClient, private router: Router) {}

  deleteCategory(): void {
    const headers = this.getAuthHeaders();

    this.http.delete(`${this.baseUrl}/${this.categoryName}`, { headers }).subscribe({
      next: () => {
        alert('Category deleted successfully!');
        this.router.navigate(['/app-admin-menu']); // Navigate to admin menu or any other route after deleting category
      },
      error: (error) => {
        if (error.status === 400 && error.error === 'Category is already in inactive state.') {
          alert('Category is already in inactive state.');
        } else {
          console.error('Error deleting category:', error);
          alert('Failed to delete category.');
        }
      }
    });
  }

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }
}