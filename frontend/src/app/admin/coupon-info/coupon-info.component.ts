import { Component, OnInit } from '@angular/core';
import { AdminHeaderFooterComponent } from '../../header-footer/admin-header-footer/admin-header-footer.component';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

interface Coupon {
  couponCode: string;
  description: string;
  discount: number;
  minOrderValue: number;
  isActive: boolean;
}

@Component({
  selector: 'app-coupon-info',
  imports: [AdminHeaderFooterComponent, FormsModule, CommonModule],
  templateUrl: './coupon-info.component.html',
  styleUrl: './coupon-info.component.css'
})
export class CouponInfoComponent implements OnInit {
  coupons: Coupon[] = [];
  filteredCoupons: Coupon[] = [];
searchTerm: string = '';

  newCoupon: Coupon = {
    couponCode: '',
    description: '',
    discount: 0,
    minOrderValue: 0,
    isActive: true
  };
  
  editingCoupon: Coupon | null = null;
  baseUrl: string = 'https://localhost:44304/api/Coupon';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.fetchCoupons();
  }

  fetchCoupons(): void {
    const headers = this.getAuthHeaders();
    this.http.get<Coupon[]>(this.baseUrl, { headers })
      .subscribe({
        next: (data) => {
          console.log('Coupons fetched successfully:', data);
          this.coupons = data;
          this.filteredCoupons = data; // Initialize filteredCoupons
        },
        error: (error) => {
          console.error('Error fetching coupons:', error);
        }
      });
  }
  

  filterCoupons(): void {
    this.filteredCoupons = this.coupons.filter(coupon =>
      coupon.couponCode.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  addCoupon(): void {
    const headers = this.getAuthHeaders();
    this.http.post<Coupon>(this.baseUrl, this.newCoupon, { headers })
      .subscribe({
        next: (data) => {
          console.log('Coupon added successfully:', data);
          this.fetchCoupons(); // Refresh the list after adding
          this.clearForm();
        },
        error: (error) => {
          console.error('Error adding coupon:', error);
        }
      });
  }

  clearForm(): void {
    this.newCoupon = {
      couponCode: '',
      description: '',
      discount: 0,
      minOrderValue: 0,
      isActive: true
    };
  }

  editCoupon(coupon: Coupon): void {
    console.log('Editing coupon:', coupon);
    this.editingCoupon = { ...coupon };
    // Scroll to the edit form
    document.querySelector('.edit-coupon-form')?.scrollIntoView({ behavior: 'smooth' });
  }
  
  updateCoupon(): void {
    if (!this.editingCoupon) return;
  
    const headers = this.getAuthHeaders();
    this.http.put(`${this.baseUrl}/${this.editingCoupon.couponCode}`, this.editingCoupon, { headers })
      .subscribe({
        next: () => {
          console.log('Coupon updated successfully');
          this.fetchCoupons(); // Refresh the list after update
          this.editingCoupon = null; // Clear the editing form
        },
        error: (error) => {
          console.error('Error updating coupon:', error);
        }
      });
  }

  deleteCoupon(couponCode: string): void {
    console.log('Deleting coupon with code:', couponCode);
    const headers = this.getAuthHeaders();
    this.http.delete(`${this.baseUrl}/${couponCode}`, { headers })
      .subscribe({
        next: () => {
          console.log('Coupon deleted successfully');
          this.fetchCoupons(); // Refresh the list after deletion
        },
        error: (error) => {
          console.error('Error deleting coupon:', error);
        }
      });
  }

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('No token found in localStorage');
    }
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }
}