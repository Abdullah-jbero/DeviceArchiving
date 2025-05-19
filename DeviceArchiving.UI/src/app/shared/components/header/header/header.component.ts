import { AccountService } from '../../../../core/services/account.service';
import { Router } from '@angular/router';
import { Component, Input, OnInit } from '@angular/core';
interface NavPage {
  label: string;
  route: string;
}

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  standalone: false,
})
export class HeaderComponent implements OnInit {

  @Input() pages: NavPage[] = [
    { label: 'الأجهزة', route: '/devices' },
    { label: 'العمليات', route: '/operations' },
    { label: 'أنواع العمليات', route: '/operation-types' },
    { label: 'الحسابات', route: '/account' },
  ];

  userName: string = 'غير معروف';
  pictureUrl: string = './1.jpg';
  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
    if (this.accountService.isAuthenticated()) {
      this.loadUserData();
    }
  }

  private loadUserData(): void {
    const userInfo = this.accountService.getUserInfo();
    console.log(userInfo);
    this.userName = userInfo.userName ?? 'غير معروف';
    this.pictureUrl = './1.jpg';
    // this.pictureUrl = userInfo.picture && userInfo.picture.length > 1
    //   ? userInfo.picture
    //   : './1.jpg';
  }



  logout(): void {
    this.accountService.logout();
  }
}