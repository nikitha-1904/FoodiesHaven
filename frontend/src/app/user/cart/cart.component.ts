import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserHeaderFooterComponent } from '../../header-footer/user-header-footer/user-header-footer.component';
import { AppComponent } from "../../app.component";
import {jwtDecode} from 'jwt-decode';

type CartItem = {
  productName: string;
  productImageUrl: string;
  productPrice: number;
  quantity: number;
  totalPrice: number;
};

@Component({
  selector: 'app-cart',
  imports: [CommonModule, UserHeaderFooterComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent implements OnInit {
  cartItems: CartItem[] = [];
  grandTotal: number = 0;

  constructor(private router: Router, private http: HttpClient) {}

  ngOnInit() {
    this.loadCart();
  }

  getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  loadCart() {
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      const username = decodedToken.sub; // Ensure the correct field is used to get the username

      const headers = this.getAuthHeaders();
      this.http.get<any>(`https://localhost:44304/api/Cart/${username}`, { headers })
        .subscribe(
          (data) => {
            if (data && Array.isArray(data.items)) {
              this.cartItems = data.items;
              this.grandTotal = data.grandTotal;
            } else {
              console.error('Data is not in the expected format:', data);
            }
          },
          (error) => {
            console.error('Error loading cart:', error);
          }
        );
    } else {
      console.error('No token found in local storage.');
    }
  }

  increaseQuantity(index: number) {
    this.cartItems[index].quantity++;
    this.updateGrandTotal();
    this.updateCartItem(this.cartItems[index].productName, this.cartItems[index].quantity);
  }
  
  decreaseQuantity(index: number) {
    if (this.cartItems[index].quantity > 1) {
      this.cartItems[index].quantity--;
      this.updateGrandTotal();
      this.updateCartItem(this.cartItems[index].productName, this.cartItems[index].quantity);
    }
  }
  

  removeCartItem(productName: string, quantity: number) {
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      const username = decodedToken.sub; // Ensure the correct field is used to get the username
  
      const headers = this.getAuthHeaders();
      this.http.delete(`https://localhost:44304/api/Cart?productName=${productName}&username=${username}&quantity=${quantity}`, { headers })
        .subscribe(
          () => {
            console.log('Cart item removed successfully.');
          },
          (error) => {
            console.error('Error removing cart item:', error);
          }
        );
    } else {
      console.error('No token found in local storage.');
    }
  }
  
// Update the removeItem method to call removeCartItem
removeItem(index: number) {
  const productName = this.cartItems[index].productName;
  this.cartItems.splice(index, 1);
  this.updateGrandTotal();
  this.removeCartItem(productName, 1);
}

  getTotal() {
    return this.cartItems.reduce((total, item) => total + item.productPrice * item.quantity, 0);
  }

  updateGrandTotal() {
    this.grandTotal = this.getTotal();
  }

  continueShopping() {
    this.router.navigate(['/app-user-menu']);
  }

  updateCartItem(productName: string, quantity: number) {
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      const username = decodedToken.sub; // Ensure the correct field is used to get the username
  
      const headers = this.getAuthHeaders();
      this.http.put(`https://localhost:44304/api/Cart?productName=${productName}&username=${username}&quantity=${quantity}`, {}, { headers })
        .subscribe(
          () => {
            console.log('Cart item updated successfully.');
          },
          (error) => {
            console.error('Error updating cart item:', error);
          }
        );
    } else {
      console.error('No token found in local storage.');
    }
  }

  checkout() {
    this.router.navigate(['/app-user-payment']);
  }

  addItemToCart(newItem: CartItem) {
    const existingItem = this.cartItems.find(item => item.productName === newItem.productName);
    if (existingItem) {
      alert('This product is already in the cart.');
    } else {
      this.cartItems.push(newItem);
      this.updateGrandTotal();
    }
  }
}