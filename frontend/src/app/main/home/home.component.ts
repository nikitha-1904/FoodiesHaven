import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  openNav(): void {
    const sidenav = document.getElementById("mySidenav");
    if (sidenav) {
      sidenav.style.width = "150px";
    }
  }
  closeNav(): void {
    const sidenav = document.getElementById("mySidenav");
    if (sidenav) {
      sidenav.style.width = "0";
    }
  }
}
