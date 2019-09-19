import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { AuthService } from './../../_services/auth.service';
import { User } from './../../_models/User';
import { Component, OnInit, Input } from '@angular/core';
  
@Component({
  selector: 'app-member-cards',
  templateUrl: './member-cards.component.html',
  styleUrls: ['./member-cards.component.css']
})
export class MemberCardsComponent implements OnInit {

  @Input() user: User;

  constructor(private authServive: AuthService, private userService: UserService, private alertifySvc: AlertifyService) { }

  ngOnInit() {
  }

  sendLike(id: number) {
    this.userService.sendLike(this.authServive.decodedToken.nameid, id).subscribe((data) => {
        this.alertifySvc.success('you have liked:' + this.user.knownAs);
    }, error => {
        this.alertifySvc.error(error);
    });
  }
}
