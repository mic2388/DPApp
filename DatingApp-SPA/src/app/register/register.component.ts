import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  model: any = {};
  //@Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();

  constructor(public authSerive: AuthService, private alertifyService: AlertifyService ) { }

  ngOnInit() {
  }


  register() {
    this.authSerive.register(this.model).subscribe(() => {
      this.alertifyService.success('Success');
      console.log('success');
    },
    (error) => {
        this.alertifyService.error('Error');
        console.log(error);
    });
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}
