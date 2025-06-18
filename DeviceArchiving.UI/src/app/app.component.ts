import { Component } from '@angular/core';
import { AccountService } from './core/services/account.service';
import { UserRole } from './core/models/authentication.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'DeviceArchiving.UI';
  pages: { label: string; route: string }[] = [];

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
    // Get user info (make sure this returns user role)
    const user = this.accountService.getUserInfo();

    // Base pages accessible to all users
    this.pages = [
      { label: 'الصفحة الرئيسة', route: '/devices' },
      { label: 'ادارة العمليات', route: '/operation-types' },
      { label: 'الأجهزة المحذوفة', route: '/devices/delete-devices' },
    ];

    // Add "إنشاء حساب" page only if user is admin
    if (user.role === UserRole.Admin) {
      this.pages.push({ label: 'إنشاء حساب', route: '/account/signup' });
    }
  }
}
