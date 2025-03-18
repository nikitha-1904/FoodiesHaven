import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-user-header-footer',
  templateUrl: './user-header-footer.component.html',
  styleUrls: ['./user-header-footer.component.css']
})
export class UserHeaderFooterComponent implements OnInit {
  username: string | null = '';

  constructor(private http: HttpClient, private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.username = localStorage.getItem('username');
  }

  logout(): void {
    localStorage.clear();
    this.router.navigate(['/app-home']);
  }
}