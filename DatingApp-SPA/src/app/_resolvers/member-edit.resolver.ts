import { AuthService } from './../_services/auth.service';
import { catchError } from 'rxjs/operators';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { User } from '../_models/User';
import { Observable, of } from 'rxjs';

@Injectable()
export class MemberEditResolver implements Resolve<User> {


 constructor(private userService: UserService, private alertifySvc: AlertifyService , private router: Router,
     private authSvc: AuthService) {}

 resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): User | Observable<User> | Promise<User> {
     return this.userService.getUser(this.authSvc.decodedToken.nameid).pipe(catchError(error => {
        this.alertifySvc.error('Error in fectching user for ' + this.authSvc.decodedToken.nameid);
        return of(null);
    }));
}

}
