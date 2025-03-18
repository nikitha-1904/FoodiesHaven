import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { UserHeaderFooterComponent } from '../../header-footer/user-header-footer/user-header-footer.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';
import { bootstrapApplication } from '@angular/platform-browser';

interface Coupon {
  couponCode: string;
  description: string;
  discount: number;
  minOrderValue: number;
  isActive: boolean;
}

interface PaymentDetailsDTO {
  cardNo?: string;
  expiryDate?: string;
  upiId?: string;
}
interface CartDTO {
  cartId: number;
  userId: number;
  items: CartItemDTO[];
  grandTotal: number;
}interface CartItemDTO {
  productName: string;
  productImageUrl: string;
  productPrice: number;
  quantity: number;
  totalPrice: number;
}

interface PaymentResultDTO {
  cartItems: CartDTO;
  isSuccess: boolean;
  status: string;
  errorMessage: string | null;
  grandTotal: number;
  discountAmount: number;
  discountedAmount: number;
  deliveryCharge: number;
  finalAmount: number;
  paymentMode: string;
  message: string;
}

@Component({
  selector: 'app-user-payment',
  templateUrl: './user-payment.component.html',
  imports: [UserHeaderFooterComponent, CommonModule, FormsModule],
  styleUrls: ['./user-payment.component.css']
})
export class UserPaymentComponent implements OnInit {
  coupons: Coupon[] = [];
  filteredCoupons: Coupon[] = [];
  searchTerm: string = '';
  pinCode="";

  newCoupon: Coupon = {
    couponCode: '',
    description: '',
    discount: 0,
    minOrderValue: 0,
    isActive: true
  };

  editingCoupon: Coupon | null = null;
  baseUrl: string = 'https://localhost:44304/api/Coupon';

  paymentMethod = '';
  address = '';
  cardNumber = '';
  expiryMonth = '';
  expiryYear = '';
  upiId = '';
  orderTotal = 0;
  couponCode = '';

  months = Array.from({ length: 12 }, (_, i) => (i + 1).toString().padStart(2, '0'));
  years = Array.from({ length: 11 }, (_, i) => (22 + i).toString());
  paymentResult: PaymentResultDTO | null = null;

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef, private router:Router) {}

  ngOnInit() {
    this.fetchCoupons();
    this.fetchOrderTotal(); // Fetch order total on initialization
    this.fetchUserAddress(); 

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
      coupon.couponCode.toLowerCase().includes(this.searchTerm.toLowerCase()) &&
      coupon.minOrderValue <= this.orderTotal // Filter by minimum order value
    );
  }

  togglePaymentMethod(method: string) {
    this.paymentMethod = this.paymentMethod === method ? '' : method;
  }
  fetchOrderTotal(): void {
    const headers = this.getAuthHeaders();
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      const username = decodedToken.sub; // Use 'sub' to get the username
  
      this.http.get<{ grandTotal: number }>(`https://localhost:44304/api/Cart/${username}`, { headers })
        .subscribe({
          next: (data) => {
            console.log('Order total fetched successfully:', data);
            this.orderTotal = data.grandTotal; // Assign grandTotal to orderTotal
          },
          error: (error) => {
            console.error('Error fetching order total:', error);
          }
        });
    } else {
      console.error('No token found in localStorage');
    }
  }
 // Update the fetchUserAddress method to include pinCode
fetchUserAddress(): void {
  const headers = this.getAuthHeaders();
  const token = localStorage.getItem('token');
  if (token) {
    const decodedToken: any = jwtDecode(token);
    const username = decodedToken.sub; // Use 'sub' to get the username

    this.http.get<{ address: string, pinCode: string }>(`https://localhost:44304/api/User/${username}`, { headers })
      .subscribe({
        next: (data) => {
          console.log('User address and pin code fetched successfully:', data);
          this.address = `${data.address}, ${data.pinCode}`; // Concatenate address and pinCode
        },
        error: (error) => {
          console.error('Error fetching user address and pin code:', error);
        }
      });
  } else {
    console.error('No token found in localStorage');
  }
}
  navigateToUserInfo(): void {
    this.router.navigate(['/app-user-info']);
  }
  selectCoupon(coupon: Coupon): void {
    this.couponCode = coupon.couponCode;
  }
  
  confirmOrder() {
    const selectedCoupon = this.coupons.find(coupon => coupon.couponCode === this.couponCode);
  
    if (selectedCoupon) {
      if (this.orderTotal < selectedCoupon.minOrderValue) {
        alert(`${this.couponCode} coupon requires a minimum order value of ${selectedCoupon.minOrderValue}.`);
        return;
      }
      const discount = selectedCoupon.discount;
      // Apply discount logic here if needed
    }
  
    let requestBody: PaymentDetailsDTO = {
      cardNo: '',
      expiryDate: '',
      upiId: ''
    };
  
    if (this.paymentMethod === 'card') {
      const cardNumberPattern = /^\d{16}$/;
      if (!cardNumberPattern.test(this.cardNumber.replace(/\s+/g, ''))) {
        alert('Please enter a valid 16-digit card number.');
        return;
      }
      requestBody.cardNo = this.cardNumber.replace(/\s+/g, '');
      requestBody.expiryDate = `${this.expiryMonth}/${this.expiryYear}`;
      requestBody.upiId = ''; // Ensure UPI ID is an empty string
    } else if (this.paymentMethod === 'upi') {
      const upiPattern = /^\d{10}@ybl$/;
      if (!upiPattern.test(this.upiId)) {
        alert('Please enter a valid UPI ID (10 digits followed by @ybl).');
        return;
      }
      requestBody.upiId = this.upiId;
      requestBody.cardNo = ''; // Ensure Card Number is an empty string
      requestBody.expiryDate = ''; // Ensure Expiry Date is an empty string
    } else if (this.paymentMethod === 'cash') {
      requestBody.cardNo = '';
      requestBody.expiryDate = '';
      requestBody.upiId = '';
    }else{
      alert('Payment Method field is required');
    }
  
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      const username = decodedToken.sub; // Use 'sub' to get the username
  
      const headers = this.getAuthHeaders();
      const checkoutUrl = `https://localhost:44304/api/Cart/Checkout?username=${username}&paymentMode=${this.paymentMethod}&couponName=${this.couponCode}`;
  
      console.log('Checkout URL:', checkoutUrl);
      console.log('Request Body:', requestBody);
      console.log('Headers:', headers);
  
      this.http.post<PaymentResultDTO>(checkoutUrl, requestBody, { headers })
      .subscribe({
        next: (response) => {
          alert('Order Confirmed!');
          console.log('Checkout successful:', response);
          this.paymentResult = response; // Assign the response to paymentResult
        },
        error: (error) => {
          console.error('Error during checkout:', error);
          alert(error.error);
          console.log('Error details:', error.error);
          alert('Error during checkout. Please try again.');
        }
      });
    } else {
      console.error('No token found in localStorage');
    }
  }
  formatCardNumber(event: any) {
    const input = event.target.value.replace(/\D/g, '').substring(0, 16); // Remove non-digits and limit to 16 digits
    const formatted = input.match(/.{1,4}/g)?.join(' ') || ''; // Add space every 4 digits
    this.cardNumber = formatted;
  }

  applyCoupon(couponCode?: string): void {
    if (couponCode) {
      this.couponCode = couponCode;
    }
    // Logic to apply the coupon
    alert(`Coupon ${this.couponCode} applied!`);
  }

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('No token found in localStorage');
    }
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }
}

bootstrapApplication(UserPaymentComponent);
