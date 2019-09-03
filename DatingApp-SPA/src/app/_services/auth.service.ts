import { User } from 'src/app/_models/User';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { logging } from 'protractor';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import {BehaviorSubject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

baseUrl = environment.apiUrl + 'auth/';
jwtHelper = new JwtHelperService();
decodedToken: any;
currentUser: User;
photoUrl = new BehaviorSubject<string>('../../assets/user.png');
currentPhotoUrl = this.photoUrl.asObservable();

constructor(private http: HttpClient) { }

changeMemberPhotoUrl(photoUrl: string) {
  this.photoUrl.next(photoUrl);
}

login(model: User) {
  return this.http.post(this.baseUrl + 'login', model).pipe(
    map((response: any) => {

      const user = response;
      if (user) {
        localStorage.setItem('token', user.token);
        localStorage.setItem('user', JSON.stringify(user.user));
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        this.currentUser = user.user;
        this.changeMemberPhotoUrl(this.currentUser.photoUrl);
      }
    })
  );
}

register(model: any) {
  return this.http.post(this.baseUrl + 'register', model);


}

loggedIn() {

  const token = localStorage.getItem('token');
  return this.jwtHelper.isTokenExpired(token);

}

}

