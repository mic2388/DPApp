import { catchError } from 'rxjs/operators';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { User } from '../_models/User';
import { Observable, of } from 'rxjs';

@Injectable()
export class MemberListResolver implements Resolve<User[]> {
  pageNumber = 1;
  pageSize = 5;


 constructor(private userService: UserService, private alertifySvc: AlertifyService , private router: Router ) {}

 resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): User[] | Observable<User[]> | Promise<User[]> {
     return this.userService.getUsers(this.pageNumber, this.pageSize).pipe(catchError(error => {
        this.alertifySvc.error('Error in fectching user');
        return of(null);
    }));
}

}
