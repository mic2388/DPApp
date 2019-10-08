import { AuthService } from './../_services/auth.service';
import { catchError } from 'rxjs/operators';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { Message } from '@angular/compiler/src/i18n/i18n_ast';

@Injectable()
export class MessagesResolver implements Resolve<Message[]> {
  pageNumber = 1;
  pageSize = 5;


 constructor(private userService: UserService, private alertifySvc: AlertifyService ,
    private router: Router, private authService: AuthService ) {}

 resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Message[] | Observable<Message[]> | Promise<Message[]> {
     return this.userService.getMessages(this.authService.decodedToken.nameid, this.pageNumber, this.pageSize)
     .pipe(catchError(error => {
        this.alertifySvc.error('Error in fectching messages');
        this.router.navigate(['/home']);
        return of(null);
    }));
}

}
