import { Component, OnInit } from '@angular/core';
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

  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
    //this.getCurrentUser();
  }

  login() {
    //console.log(this.model);
    this.accountService.login(this.model).subscribe(reponse => {
      console.log(reponse);
    }, error => {
      console.log(error);
    });
  }

  logout() {
    this.accountService.logout();
  }

  //getCurrentUser() {
  //  this.accountService.currentUser$.subscribe(user => {
  //    this.loggedIn = !!user;
  //  }, error => {
  //    console.log(error);
  //  })
  //}

}
