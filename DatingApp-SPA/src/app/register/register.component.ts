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

  constructor(private authSerive: AuthService) { }

  ngOnInit() {
  }


  register() {
    this.authSerive.register(this.model).subscribe(() => {
      console.log('success');
    },
    () => {
        console.log('failure');
    });
  }

  cancel(){
    this.cancelRegister.emit(false);
    console.log('cancel');
  }

}
