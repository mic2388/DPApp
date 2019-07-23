import { AuthService } from './../_services/auth.service';
import { Component, OnInit } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  constructor(public authService: AuthService,
     private alertifyService: AlertifyService , private router: Router) { }

  ngOnInit() {
  }

  login() {
      this.authService.login(this.model).subscribe(next => {
        console.log('successfully logged in');
        this.alertifyService.success('successfully logged in');
      }, error => {
        this.alertifyService.error('login failure');
      console.log(error);
      }, () => {
         this.router.navigate(['/members']);
      });

  }

  loggedIn() {
    // const token = localStorage.getItem('token');
    // return !!token;
    return this.authService.loggedIn();
  }

  logOut() {
    localStorage.removeItem('token');
    this.alertifyService.message('successfully logged out');
    this.router.navigate(['/home']);
    console.log('logged out');
  }

}
