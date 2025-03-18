import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserHeaderFooterComponent } from '../../header-footer/user-header-footer/user-header-footer.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-contact',
  imports: [UserHeaderFooterComponent, FormsModule, CommonModule],
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent {
  fullName: string = '';
  email: string = '';
  subject: string = '';
  query: string = '';

  constructor(private http: HttpClient) {}

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('No token found in localStorage');
    }
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }

  submitFeedback() {
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      const username = decodedToken.sub; // Use 'sub' to get the username

      const headers = this.getAuthHeaders();
      const url = `https://localhost:44304/api/Feedback/${username}`;

      const feedbackDTO = {
        name: this.fullName,
        email: this.email,
        subject: this.subject,
        message: this.query
      };

      this.http.post(url, feedbackDTO, { headers })
        .subscribe({
          next: (response) => {
            console.log('Feedback submitted successfully:', response);
            // Optionally, reset the form fields
            this.fullName = '';
            this.email = '';
            this.subject = '';
            this.query = '';
          },
          error: (error) => {
            console.error('Error submitting feedback:', error);
            alert(error.error);
          }
        });
    } else {
      console.error('No token found in localStorage');
    }
  }
}