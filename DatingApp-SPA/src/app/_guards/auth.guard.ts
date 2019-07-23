import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Injectable } from '@angular/core';
import { CanActivate, Router} from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router, private alertifyService: AlertifyService) {


  }
  canActivate(): Observable<boolean> | Promise<boolean> | boolean {
    if (!this.authService.loggedIn()) {
        return true;
    }

    this.alertifyService.error('Error, no access to said , routes !!!');
    this.router.navigate(['/home']);
    return false;
  }
}
