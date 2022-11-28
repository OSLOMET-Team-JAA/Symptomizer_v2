import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {HttpClient} from "@angular/common/http";
import {User} from "../User";


@Component({
  templateUrl: 'login.component.html',
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  id: number = 0;
  username: string = "";
  password: any = "";
  salt: any = "";

  constructor(
      private http: HttpClient,
      private fb: FormBuilder,
      private route: ActivatedRoute,
      private router: Router) {
    this.loginForm = this.fb.group({
        username: [null, [Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,30}")]],
        password: [null, [Validators.required, Validators.pattern("(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}")]]
    });
  }

  ngOnInit() {
    this.loginForm = this.fb.group({
      username: [null, [Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,30}")]],
      password: [null, [Validators.required, Validators.pattern("(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}")]]
    });
  }

  onSubmit() {
      this.loginSubmission();
    }
    
    loginSubmission(){
        const user = new User();
        user.username = this.loginForm.value.username;
        user.password = this.loginForm.value.password;

        console.log(user)
        this.http.post("api/Patient/login", user)
            .subscribe(resp => {
              console.log(resp)
              this.router.navigate(['/home']);
            },
                error => console.log(error));
    }
}
