import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminHeaderFooterComponent } from '../../header-footer/admin-header-footer/admin-header-footer.component';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';

interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  isActive: boolean;
  dateAdded: string;
  categoryId: number;
  categoryName: string;
  vegNonVeg: string; // Ensure this property is included
  isEditing?: boolean;
}

interface Category {
  categoryId: number;
  name: string;
  isActive: boolean;
}

@Component({
  selector: 'app-add-view-products',
  imports: [FormsModule, AdminHeaderFooterComponent, CommonModule],
  templateUrl: './add-view-products.component.html',
  styleUrls: ['./add-view-products.component.css']
})
export class AddViewProductsComponent implements OnInit {
  product: Product = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    imageUrl: '',
    isActive: false,
    dateAdded: '',
    categoryId: 0,
    categoryName: '',
    vegNonVeg: ''
  };

  categories: Category[] = [];
  products: Product[] = [];
  searchTerm: string = '';
  selectedType: string = '';
  baseUrl: string = 'https://localhost:44304/api/Products'; // Add this line

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit() {
    this.fetchProducts();
    this.fetchCategories();
  }

  fetchCategories(): void {
    const headers = this.getAuthHeaders();
    this.http.get<Category[]>('https://localhost:44304/api/Category', { headers })
      .subscribe({
        next: (data) => {
          console.log('Categories fetched successfully:', data);
          this.categories = data;
        },
        error: (error) => {
          console.error('Error fetching categories:', error);
        }
      });
  }

  addProduct(): void {
    const headers = this.getAuthHeaders();
    const categoryId = Number(this.product.categoryId); // Convert categoryId to number
    console.log('Selected category ID:', categoryId);
    console.log('Available categories:', this.categories);
    const selectedCategory = this.categories.find(category => category.categoryId === categoryId);
    if (selectedCategory) {
      this.product.categoryName = selectedCategory.name;
    } else {
      console.error('Selected category not found');
      alert('Selected category not found. Please select a valid category.');
      return;
    }
    console.log('Product to be added:', this.product); // Add this line
    this.http.post<Product>(this.baseUrl, this.product, { headers }).subscribe({
      next: (data) => {
        console.log('Product added:', data);
        alert('Product added successfully!');
        this.fetchProducts(); // Fetch the updated list of products
        this.clearForm();
      },
      error: (error) => {
        console.error('Error adding product:', error);
        alert(error.error);
      }
    });
  }

  fetchProducts() {
    const headers = this.getAuthHeaders();
    this.http.get<Product[]>('https://localhost:44304/api/Products', { headers })
      .subscribe(data => {
        this.products = data.map(product => {
          const category = this.categories.find(cat => cat.categoryId === product.categoryId);
          if (category) {
            product.categoryName = category.name;
          }
          return product;
        });
      }, error => {
        console.error('Error fetching products', error);
      });
  }

  onSubmit() {
    this.products.push({ ...this.product, id: this.products.length + 1, dateAdded: new Date().toISOString() });
    this.clearForm();
  }

  clearForm() {
    this.product = {
      id: 0,
      name: '',
      description: '',
      price: 0,
      imageUrl: '',
      isActive: false,
      dateAdded: '',
      categoryId: 0,
      categoryName: '',
      vegNonVeg: ''
    };
  }

  get filteredProducts() {
    return this.products.filter(product =>
      product.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      product.categoryName.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  trackByProductId(index: number, product: Product): number {
    return product.id;
  }

  toggleVegNonVeg(type: string) {
    this.selectedType = type;
  }

  editProduct(product: Product) {
    this.router.navigate(['/app-edit-product', product.name]);
  }

  deleteProduct(product: Product) {
    if (!product.isActive) {
      alert(`${product.name} is already in inactive status`);
    } else {
      const headers = this.getAuthHeaders();
      product.isActive = false;
      this.http.put(`${this.baseUrl}/${product.name}`, product, { headers }).subscribe({
        next: () => {
          alert(`${product.name} has been set to inactive status.`);
          this.fetchProducts(); // Refresh the product list
        },
        error: (error) => {
          console.error('Error updating product status:', error);
          alert('Error updating product status. Please try again.');
        }
      });
    }
  }

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('No token found in localStorage');
    }
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }
}