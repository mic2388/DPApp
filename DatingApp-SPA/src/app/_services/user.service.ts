import { AlertifyService } from './alertify.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/User';
import { map } from 'rxjs/operators';

const httpOptions  = {
  headers : new HttpHeaders({
    'Authorization': 'Bearer ' + localStorage.getItem('token')
  })
};


@Injectable({
  providedIn: 'root'
})
export class UserService {
baseUrl: string = environment.apiUrl;
constructor(private http: HttpClient) { }

getUsers(): Observable<User[]> {
return this.http.get<User[]>(this.baseUrl + 'users');
// return this.http.get<User[]>(this.baseUrl + 'users', httpOptions);
}

getUser(id: number): Observable<User> {
  return this.http.get<User>(this.baseUrl + 'users/' + id);
  // return this.http.get<User>(this.baseUrl + 'users/' + id, httpOptions);
}

updateUser(id: number, user: User) {
  return this.http.put(this.baseUrl + 'users/' + id, user);
}

setMainPhoto(id: number, userId: number) {
  return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/SetMain', {})
  .pipe(map((response: any) => {
      console.log('Successfull');
      return 'successfull';
  }));
}

deletePhoto(id: number, userId: number) {
  return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id).pipe(map((repsonse: any) => {
    console.log(repsonse);
  }, (error: any) => {

  }));
}
}
