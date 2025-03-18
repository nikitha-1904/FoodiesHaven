import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-update-category',
  imports: [CommonModule, FormsModule],
  templateUrl: './update-category.component.html',
  styleUrls: ['./update-category.component.css']
})
export class UpdateCategoryComponent implements OnInit {
  categoryName: string = '';
  category: any = null;

  private baseUrl = 'https://localhost:44304/api/Category';

  constructor(private http: HttpClient, private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.categoryName = this.route.snapshot.paramMap.get('name') || '';
    if (this.categoryName) {
      this.fetchCategory();
    }
  }

  fetchCategory(): void {
    const headers = this.getAuthHeaders();
    this.http.get<any>(`${this.baseUrl}/${this.categoryName}`, { headers }).subscribe({
      next: (data) => {
        this.category = data;
      },
      error: (error) => {
        console.error('Error fetching category:', error);
        alert('Failed to fetch category.');
      }
    });
  }

  updateCategory(): void {
    const headers = this.getAuthHeaders();
    const requestBody = { name: this.category.name, isActive: this.category.isActive };

    this.http.put(`${this.baseUrl}/${this.categoryName}`, requestBody, { headers }).subscribe({
      next: () => {
        alert('Category updated successfully!');
        this.router.navigate(['/app-admin-menu']); // Navigate to admin menu or any other route after updating category
      },
      error: (error) => {
        console.error('Error updating category:', error);
        alert('Failed to update category.');
      }
    });
  }

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }
}