import { Routes } from '@angular/router';
import { AdminLoginComponent } from './admin/admin-login/admin-login.component';
import { UserRegistrationComponent } from './user/user-registration/user-registration.component';
import { UserLoginComponent } from './user/user-login/user-login.component';
import { HomeComponent } from './main/home/home.component';
import { ContactComponent } from './main/contact/contact.component';
import { AboutComponent } from './main/about/about.component';
import { ContactInfoComponent } from './admin/contact-info/contact-info.component';
import { UsersAdminInfoComponent } from './admin/users-admin-info/users-admin-info.component';
import { AddViewProductsComponent } from './admin/add-view-products/add-view-products.component';
import { PaymentHistoryComponent } from './admin/payment-history/payment-history.component';
import { CartComponent } from './user/cart/cart.component';
import { DashboardComponent } from './admin/dashboard/dashboard.component';
import { AdminMenuComponent } from './admin/admin-menu/admin-menu.component';
import { EditProductComponent } from './admin/edit-product/edit-product.component';
import { UserMenuComponent } from './user/user-menu/user-menu.component';
import { UserInfoComponent } from './user/user-info/user-info.component';
import { AddCategoryComponent } from './admin/category/add-category/add-category.component';
import { UpdateCategoryComponent } from './admin/category/update-category/update-category.component';
import { DeleteCategoryComponent } from './admin/category/delete-category/delete-category.component';
import { UserPaymentComponent } from './user/user-payment/user-payment.component';
import { CouponInfoComponent } from './admin/coupon-info/coupon-info.component';
import { OrderHistoryComponent } from './user/order-history/order-history.component';
import { LogoutComponent } from './admin/logout/logout.component';
export const routes: Routes = [
  
  { path: 'app-admin-login', component: AdminLoginComponent },
  { path: 'app-user-registration', component: UserRegistrationComponent},
  { path: 'app-user-login', component: UserLoginComponent },
  { path: '', redirectTo: '/app-home', pathMatch: 'full' },
  { path: 'app-home', component:HomeComponent },
  { path: 'app-home', component:HomeComponent},
  { path: 'app-contact', component:ContactComponent},
  { path: 'app-about', component:AboutComponent},
  { path: 'app-contact-info', component:ContactInfoComponent},
  { path: 'app-users-admin-info', component:UsersAdminInfoComponent},
  { path: 'app-add-view-products', component:AddViewProductsComponent},
  { path: 'app-payment-history', component:PaymentHistoryComponent},
  { path: 'app-cart', component:CartComponent},
  { path: 'app-dashboard', component:DashboardComponent},
  { path: 'app-admin-menu', component:AdminMenuComponent},
  { path: 'app-edit-product/:name', component: EditProductComponent }, 
  { path: 'app-user-menu', component:UserMenuComponent},
  { path: 'app-user-info', component:UserInfoComponent},
  { path: 'app-add-category',component:AddCategoryComponent},
  { path: 'app-delete-category/:name',component:DeleteCategoryComponent},
  { path: 'app-update-category/:name',component:UpdateCategoryComponent},
  { path: 'app-user-payment', component:UserPaymentComponent},
  { path:'app-coupon-info',component:CouponInfoComponent},
  { path:'app-order-history', component:OrderHistoryComponent},
  { path:'app-logout',component:LogoutComponent}
];
