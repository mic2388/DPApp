import { PaginationResult } from './../_models/Pagination';
import { AlertifyService } from './alertify.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
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

getUsers(page?, itemsPerPage?, userParams?, likesParams?): Observable<PaginationResult<User[]>> {
const paginatedResult: PaginationResult<User[]> =  new PaginationResult<User[]>();
let params = new HttpParams();

if (page != null && itemsPerPage != null) {
  params = params.append('pageNumber', page);
  params = params.append('pageSize', itemsPerPage);
}
if (userParams != null) {
  params = params.append('minAge', userParams.minAge);
  params = params.append('maxAge', userParams.maxAge);
  params = params.append('gender', userParams.gender);
  params = params.append('orderBy', userParams.orderBy);
}
if (likesParams === 'Likers') {
  params = params.append('likers', 'true');
}

if (likesParams === 'Likees') {
  params = params.append('likees', 'true');
}

return this.http.get<User[]>(this.baseUrl + 'users', {observe: 'response', params}).pipe(map(
  response => {
      paginatedResult.result = response.body;
      if (response.headers.get('Pagination') != null) {
      paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
  })
  );
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

sendLike(id: number, recipientId: number) {
  return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
}

}
