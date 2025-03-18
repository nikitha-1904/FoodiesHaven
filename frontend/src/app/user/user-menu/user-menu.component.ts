import { Component, OnInit } from '@angular/core';
import { UserHeaderFooterComponent } from '../../header-footer/user-header-footer/user-header-footer.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import {jwtDecode} from 'jwt-decode';

interface Product {
  productId: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  isActive: boolean;
  createdDate: string;
  vegNonVeg: string;
  categoryId: number;
}

interface Category {
  categoryId: number;
  name: string;
  isActive: boolean;
  products: Product[];
}

interface CartItemDTO {
  productName: string;
  productImageUrl: string;
  productPrice: number;
  quantity: number;
  totalPrice: number;
}

@Component({
  selector: 'app-user-menu',
  imports: [UserHeaderFooterComponent, CommonModule, FormsModule],
  templateUrl: './user-menu.component.html',
  styleUrls: ['./user-menu.component.css']
})
export class UserMenuComponent implements OnInit {
  categories: Category[] = [];
  products: Product[] = [];
  selectedCategory = 'All';
  vegOnly = false;
  nonVegOnly = false;
  private baseUrl = 'https://localhost:44304/api';
  cart: Product[] = [];

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit(): void {
    this.fetchCategories();
    this.fetchProducts();
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }

  fetchCategories(): void {
    const headers = this.getAuthHeaders();
    console.log('Fetching categories with headers:', headers);
    this.http.get<Category[]>(`${this.baseUrl}/Category`, { headers }).subscribe({
      next: (categories) => {
        console.log('Categories fetched successfully:', categories);
        this.categories = [...categories];
      },
      error: (error) => {
        console.error('Error fetching categories:', error);
      }
    });
  }

  // fetchProducts(): void {
  //   const headers = this.getAuthHeaders();
  //   console.log('Fetching products with headers:', headers);
  //   this.http.get<Product[]>(`${this.baseUrl}/Product`, { headers }).subscribe({
  //     next: (products) => {
  //       console.log('Products fetched successfully:', products);
  //       this.products = products;
  //       this.updateAllCategoryProducts();
  //     },
  //     error: (error) => {
  //       console.error('Error fetching products:', error);
  //     }
  //   });
  // }
fetchProducts(): void {
  const headers = this.getAuthHeaders();
  console.log('Fetching products with headers:', headers);
  this.http.get<Product[]>(`${this.baseUrl}/Products`, { headers }).subscribe({
    next: (products) => {
      console.log('Products fetched successfully:', products);
      this.products = products;
      this.updateAllCategoryProducts();
    },
    error: (error) => {
      console.error('Error fetching products:', error);
    }
  });
}
  updateAllCategoryProducts(): void {
    const allCategory = this.categories.find(category => category.categoryId === 0);
    if (allCategory) {
      allCategory.products = this.products;
    }
  }

  fetchCategoryByName(categoryName: string): void {
    const headers = this.getAuthHeaders();
    console.log(`Fetching category ${categoryName} with headers:`, headers);
    this.http.get<Category>(`${this.baseUrl}/Category/${categoryName}`, { headers }).subscribe({
      next: (category) => {
        console.log(`Category ${categoryName} fetched successfully:`, category);
        this.products = category.products;
      },
      error: (error) => {
        console.error(`Error fetching category ${categoryName}:`, error);
      }
    });
  }

  selectCategory(category: Category): void {
    this.selectedCategory = category.name;
    if (category.name === "All") {
      this.fetchProducts(); // Fetch all products if "All" category is selected
    } else {
      this.fetchCategoryByName(category.name); // Fetch products for the selected category
    }
  }

  toggleVegNonVeg(event: any): void {
    this.vegOnly = event.target.checked;
    this.nonVegOnly = !event.target.checked; // Update non-veg flag based on veg toggle
    this.filterProducts();
  }

  filterProducts(): void {
    if (this.selectedCategory === 'All') {
      this.fetchProducts(); // Fetch all products if "All" category is selected
    } else {
      this.fetchCategoryByName(this.selectedCategory); // Fetch products for the selected category
    }
  }

  filteredProducts(): Product[] {
    if (
      this.selectedCategory.toLowerCase() === 'desserts' || 
      this.selectedCategory.toLowerCase() === 'milkshakes' || 
      this.selectedCategory.toLowerCase() === 'beverages'
    ) {
      // If category is one of the specified categories, return all products in that category
      return this.products;
    } else if (this.vegOnly) {
      // If vegOnly is true, return only veg products and products with null vegNonVeg
      return this.products.filter(product => product.vegNonVeg === null || product.vegNonVeg.toLowerCase() === 'veg');
    } else if (this.nonVegOnly) {
      // If nonVegOnly is true, return only non-veg products and products with null vegNonVeg
      return this.products.filter(product => product.vegNonVeg === null || product.vegNonVeg.toLowerCase() === 'nonveg');
    } else {
      // Otherwise, return all products
      return this.products;
    }
  }

  addToCart(product: Product): void {
    if (product.isActive) {
      console.log('Adding to cart:', product);
      const headers = this.getAuthHeaders();
      const token = localStorage.getItem('token');
      if (token) {
        const decodedToken: any = jwtDecode(token);
        const username = decodedToken.sub; // Ensure the correct field is used to get the username
        const url = `https://localhost:44304/api/Cart?productName=${encodeURIComponent(product.name)}&username=${encodeURIComponent(username)}`;
        this.http.post<CartItemDTO>(url, {}, { headers }).subscribe({
          next: (cartItem) => {
            console.log('Product added to cart successfully:', cartItem);
            this.cart.push(product);
            console.log('Current cart:', this.cart);
            this.navigateTo('app-cart'); // Navigate to the cart page
          },
          error: (error) => {
            console.error('Error adding product to cart:', error);
            alert(error.error); // Display the error message as an alert
          }
        });
      } else {
        console.error('Token not found.');
      }
    } else {
      alert('Product is inactive and cannot be added to the cart: ' + product.name);
    }
  }
  
  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    console.log('Token:', token);
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }
}