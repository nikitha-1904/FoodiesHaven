import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { AdminHeaderFooterComponent } from '../../header-footer/admin-header-footer/admin-header-footer.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-contact-info',
  templateUrl: './contact-info.component.html',
  imports: [CommonModule, AdminHeaderFooterComponent, FormsModule],
  styleUrls: ['./contact-info.component.css']
})
export class ContactInfoComponent implements OnInit {
  queries: any[] = [];
  filteredQueries: any[] = [];

  constructor(private http: HttpClient) {}
  private baseUrl = 'https://localhost:44304/api';

  ngOnInit(): void {
    this.getQueries();
  }

  getQueries(): void {
    const headers = this.getAuthHeaders();
    this.http.get<any[]>(`${this.baseUrl}/Feedback`, { headers }).subscribe(data => {
      this.queries = data;
      this.filteredQueries = data;
    });
  }

  filterQueriesBySubject(event: Event): void {
    const selectedSubject = (event.target as HTMLSelectElement).value;
    if (selectedSubject) {
      this.filteredQueries = this.queries.filter(query =>
        query.subject.toLowerCase() === selectedSubject.toLowerCase()
      );
    } else {
      this.filteredQueries = this.queries;
    }
  }

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' });
  }
}
