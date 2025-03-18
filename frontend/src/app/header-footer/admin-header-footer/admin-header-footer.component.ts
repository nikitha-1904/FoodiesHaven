import { Component } from '@angular/core';
import { Router,NavigationEnd } from '@angular/router';

interface Coupon {
  couponCode: string;
  description: string;
  discount: number;
  minOrderValue: number;
  isActive: boolean;
}

@Component({
  selector: 'app-admin-header-footer',
  imports: [],
  templateUrl: './admin-header-footer.component.html',
  styleUrl: './admin-header-footer.component.css'
})
export class AdminHeaderFooterComponent {
  constructor(private router: Router) {}

  ngOnInit(): void {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.highlightActiveLink();
      }
    });
  }

  highlightActiveLink(): void {
    const navLinks = document.querySelectorAll('.header nav a');
    const currentUrl = this.router.url;

    navLinks.forEach(link => {
      if (link.getAttribute('href') === currentUrl) {
        link.classList.add('active');
      } else {
        link.classList.remove('active');
      }
    });
  }
  logout(): void {
    localStorage.clear();
    this.router.navigate(['/app-home']);
}
}

