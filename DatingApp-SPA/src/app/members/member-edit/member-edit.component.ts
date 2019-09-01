import { AuthService } from './../../_services/auth.service';
import { UserService } from './../../_services/user.service';
import { User } from './../../_models/User';
import { AlertifyService } from './../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  @ViewChild('editForm') editForm: NgForm;
  user: User;
  photoUrl: string;
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(private routes: ActivatedRoute, private alertifySvc: AlertifyService,
    private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
      this.routes.data.subscribe(data => {
        this.user = data['users'];
      });

      this.authService.photoUrl.subscribe(url => this.photoUrl = url);
  }

  updateUser() {
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
      this.alertifySvc.success('Updated User Profile Successfully !');
      this.editForm.reset(this.user);
    }, error => {
      this.alertifySvc.error(error);
    });

  }

  updateMainPhoto(photoUrl: string) {
    this.user.photoUrl = photoUrl;
  }
}
