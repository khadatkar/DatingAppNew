import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}
  //loggedIn: boolean;

  constructor(public accountService: AccountService, private router: Router,private toastr:ToastrService) { }

  ngOnInit(): void {
    //this.getCurrentUser();
  }

  login() {
    //console.log(this.model);
    this.accountService.login(this.model).subscribe({
      next: _ => {
        this.router.navigateByUrl('/members');
        this.model = {};
      }
    })

    //this.accountService.login(this.model).subscribe(reponse => {
    //  this.router.navigateByUrl('/members');
    //  this.model = {};
    //});
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  //getCurrentUser() {
  //  this.accountService.currentUser$.subscribe(user => {
  //    this.loggedIn = !!user;
  //  }, error => {
  //    console.log(error);
  //  })
  //}

}
