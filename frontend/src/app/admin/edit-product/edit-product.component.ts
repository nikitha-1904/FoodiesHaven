import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  isActive: boolean;
  dateAdded: string;
  category: string;
  categoryId: number;
  vegNonVeg: string;
}

interface Category {
  id: number;
  name: string;
  isActive: boolean;
}

@Component({
  selector: 'app-edit-product',
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-product.component.html',
  styleUrls: ['./edit-product.component.css']
})
export class EditProductComponent implements OnInit {
  product: Product = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    imageUrl: '',
    isActive: false,
    dateAdded: '',
    category: '',
    categoryId: 0,
    vegNonVeg: ''
  };

  categories: Category[] = [];
  baseUrl: string = 'https://localhost:44304/api/Products';
  productName: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.productName = this.route.snapshot.paramMap.get('name') || '';
    console.log('Product Name:', this.productName);
    if (this.productName) {
      this.fetchProduct();
    }
    this.fetchCategories();
  }

  fetchCategories(): void {
    const headers = this.getAuthHeaders();
    this.http.get<Category[]>('https://localhost:44304/api/Category', { headers })
      .subscribe({
        next: (data: Category[]) => {
          this.categories = data;
          this.setCategoryName(); // Set the category name after fetching categories
        },
        error: (error: any) => {
          console.error('Error fetching categories:', error);
        }
      });
  }

  fetchProduct(): void {
    const headers = this.getAuthHeaders();
    this.http.get<Product>(`${this.baseUrl}/${this.productName}`, { headers }).subscribe({
      next: (data: Product) => {
        console.log('Fetched Product:', data);
        this.product = data;
        this.product.categoryId = Number(this.product.categoryId); // Ensure categoryId is a number
        this.setCategoryName(); // Set the category name
      },
      error: (error: any) => {
        console.error('Error fetching product:', error);
        alert('Failed to fetch product.');
      }
    });
  }

  setCategoryName(): void {
    const category = this.categories.find(cat => cat.id === this.product.categoryId);
    if (category) {
      this.product.category = category.name;
    }
  }

  saveProduct(): void {
    const headers = this.getAuthHeaders();
    this.getCategoryIdByName(this.product.category).subscribe({
      next: (categoryId: number) => {
        this.product.categoryId = categoryId;
        const productDTO = {
          ...this.product,
          categoryId: Number(this.product.categoryId) // Ensure categoryId is a number
        };
        console.log('Product to be updated:', productDTO); // Add this line
        this.http.put(`${this.baseUrl}/${this.product.name}`, productDTO, { headers }).subscribe({
          next: () => {
            console.log('Product updated:', productDTO);
            alert('Product updated successfully!');
            this.router.navigate(['/app-add-view-products']);
          },
          error: (error: any) => {
            console.error('Error updating product:', error);
            alert('Error updating product. Please check the input values.');
            console.log('Validation errors:', error.error.errors);
          }
        });
      },
      error: (error: any) => {
        console.error('Error fetching category ID:', error);
        alert('Failed to fetch category ID.');
      }
    });
  }

  getCategoryIdByName(categoryName: string): Observable<number> {
    const headers = this.getAuthHeaders();
    return this.http.get<Category[]>(`https://localhost:44304/api/Category?name=${categoryName}`, { headers })
      .pipe(
        map((categories: Category[]) => {
          const category = categories.find(cat => cat.name === categoryName);
          return category ? category.id : 0;
        })
      );
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }
}
