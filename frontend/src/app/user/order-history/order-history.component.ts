import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserHeaderFooterComponent } from '../../header-footer/user-header-footer/user-header-footer.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { jwtDecode } from 'jwt-decode';

interface Order {
  productId: number;
  productName: string;
  quantity: number;
  orderDate: Date;
}

interface OrderGroup {
  orderNo: string;
  orders: Order[];
}

@Component({
  selector: 'app-order-history',
  imports: [UserHeaderFooterComponent, FormsModule, CommonModule],
  templateUrl: './order-history.component.html',
  styleUrls: ['./order-history.component.css']
})
export class OrderHistoryComponent implements OnInit {
  orders: OrderGroup[] = [];
  filteredOrders: OrderGroup[] = [];
  searchOrderNo: string = '';
  searchDate: string = '';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.fetchOrderHistory();
  }

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('No token found in localStorage');
    }
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }

  fetchOrderHistory() {
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      const username = decodedToken.sub; // Use 'sub' to get the username
  
      const headers = this.getAuthHeaders();
      const url = `https://localhost:44304/api/Orders/GetUserOrderHistory/${username}`;
  
      this.http.get<{ userId: number, username: string, orders: OrderGroup[] }>(url, { headers })
        .subscribe({
          next: (response) => {
            this.orders = response.orders.sort((a, b) => new Date(b.orders[0].orderDate).getTime() - new Date(a.orders[0].orderDate).getTime());
            this.filteredOrders = this.orders;
          },
          error: (error) => {
            console.error('Error fetching order history:', error);
          }
        });
    } else {
      console.error('No token found in localStorage');
    }
  }

  filterOrders() {
    this.filteredOrders = this.orders.filter(orderGroup => {
      const matchesOrderNo = orderGroup.orderNo.toLowerCase().includes(this.searchOrderNo.toLowerCase());
      const matchesDate = this.searchDate ? orderGroup.orders.some(order => new Date(order.orderDate).toISOString().split('T')[0] === this.searchDate) : true;
      return matchesOrderNo && matchesDate;
    });
  }
}