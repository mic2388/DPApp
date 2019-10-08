import { AuthService } from './../_services/auth.service';
import { UserService } from './../_services/user.service';
import { AlertifyService } from './../_services/alertify.service';
import { Pagination, PaginationResult } from './../_models/Pagination';
import { Message } from './../_models/Message';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';
  constructor(private route: ActivatedRoute, private alertifySvc: AlertifyService,
    private userService: UserService, private authService: AuthService) {
     }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.messages = data['messages'].result;
      this.pagination = data['messages'].pagination;
    });
  }


  loadMessages() {
    this.userService.getMessages(this.authService.decodedToken.nameid,
      this.pagination.currentPage, this.pagination.itemsPerPage, this.messageContainer)
      .subscribe((result: PaginationResult<Message[]>) => {
         this.messages = result.result;
         this.pagination = result.pagination;
      }, error => {
        this.alertifySvc.error(error);
      });
  }

  deleteMessage(id: number) {

    this.alertifySvc.confirm('are you sure to delete this ?',() => {
      this.userService.deleteMessage(this.authService.decodedToken.nameid, id)
      .subscribe(() => {
          this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
          this.alertifySvc.success('deleted the message successfully.');
      }, (error) => {
        this.alertifySvc.error('failed to delete message');
      });
    });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

}

