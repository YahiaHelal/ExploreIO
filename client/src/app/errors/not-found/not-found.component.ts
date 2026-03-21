import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.css'],
})
export class NotFoundComponent implements OnInit {
  pageType: string = 'Home';
  constructor(private router: Router, private accountService: AccountService) { }

  ngOnInit(): void {
    this.checkAuth();
  }

  checkAuth() {
    this.accountService.currentUser$.subscribe(user => {
      if(user) {
        this.pageType = 'Followings';
      }else {
        this.pageType = 'Home';
      }
    })
  }

  navigate() {
    this.accountService.currentUser$.subscribe(user => {
      if(user) {
        this.router.navigateByUrl('/members');
      }else {
        this.router.navigateByUrl('/');
      }
    }, err => {
      console.log(err);
    })
  }
}
