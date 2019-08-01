import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../../_services/alertify.service';
import { UserService } from '../../_services/user.service';
import { User } from '../../_models/User';
import { Component, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  users: User[];
  constructor(private userService: UserService, private alertifySvc: AlertifyService, private route: ActivatedRoute) {

   }

  ngOnInit() {

     this.route.data.subscribe(data => {
        this.users = data['users'];
     });
    // this.loadUsers();
  }

  // loadUsers(){
  //   this.userService.getUsers().subscribe((userResult: User[]) => {
  //     this.users = userResult;
  //   }, error => {
  //     this.alertifySvc.error('couldnt fetch users');
  //   });
  // }

}
