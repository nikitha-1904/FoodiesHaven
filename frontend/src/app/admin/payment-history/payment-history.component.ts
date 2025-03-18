import { Component, OnInit } from '@angular/core';
import { AdminHeaderFooterComponent } from '../../header-footer/admin-header-footer/admin-header-footer.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

interface Payment {
  paymentId: string;
  paymentMode: string;
  cardNo: string;
  expiryDate: string;
  upiId: string;
  username: string;
  userId: number;
  finalAmount: number;
  paymentStatus: string;
}

@Component({
  selector: 'app-payment-history',
  imports: [AdminHeaderFooterComponent, CommonModule, FormsModule],
  templateUrl: './payment-history.component.html',
  styleUrls: ['./payment-history.component.css']
})
export class PaymentHistoryComponent implements OnInit {
  payments: Payment[] = [];
  filteredPayments: Payment[] = [];
  selectedPaymentMode: string = '';
  selectedPaymentStatus: string = '';
  selectedUsername: string = '';
  baseUrl: string = 'https://localhost:44304/api/Payment';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.fetchPayments();
  }

  fetchPayments(): void {
    const headers = this.getAuthHeaders();
    this.http.get<Payment[]>(this.baseUrl, { headers }).subscribe({
      next: (data) => {
        this.payments = data;
        this.filteredPayments = data;
      },
      error: (error) => {
        console.error('Error fetching payments:', error);
      }
    });
  }

  filterPayments(): void {
    this.filteredPayments = this.payments.filter(payment => {
      return (this.selectedPaymentMode === '' || payment.paymentMode.toLowerCase() === this.selectedPaymentMode.toLowerCase()) &&
             (this.selectedPaymentStatus === '' || payment.paymentStatus.toLowerCase() === this.selectedPaymentStatus.toLowerCase()) &&
             (this.selectedUsername === '' || payment.username.toLowerCase().includes(this.selectedUsername.toLowerCase()));
    });
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }
}