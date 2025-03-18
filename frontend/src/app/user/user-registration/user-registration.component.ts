import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormBuilder,FormGroup,ReactiveFormsModule,Validators, FormControl, FormArray } from '@angular/forms';
@Component({
  selector: 'app-user-registration',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-registration.component.html',
  styleUrl: './user-registration.component.css',
  standalone:true
})
export class UserRegistrationComponent {

  onLogin() {
    throw new Error('Method not implemented.');
    }
      registrationForm: FormGroup;
      errormessage: string='';
     
      constructor(private authService: AuthService, private router: Router, private fb: FormBuilder) {
        this.registrationForm = this.fb.group({
          name: ['', Validators.required],
          username: ['', Validators.required],
          mobile: ['', Validators.required],
          email: ['', [Validators.required, Validators.email]],
          address: this.fb.array([this.fb.control('', Validators.required)]),
          pinCode: ['', Validators.required],
          password: ['', Validators.required],
          role: ['user', Validators.required],
          createdDate: [new Date(), Validators.required],
        });
      }
     
      onRegister(): void {
        if (this.registrationForm.valid) {
          console.log(this.registrationForm.value);
          this.authService.register(this.registrationForm.value).subscribe(
            response => {
              console.log('Registration successful', response);
              this.router.navigate(['/app-user-login']);
            },
            error => {
              console.error('Registration failed', error);
            }
          );
        }
      }

      get addressControls() {
        return (this.registrationForm.get('address') as FormArray).controls;
      }
}

