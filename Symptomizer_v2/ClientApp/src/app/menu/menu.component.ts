import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Router} from "@angular/router";
import {User} from "../User";

@Component({
    selector: 'app-menu',
    templateUrl: './menu.component.html'
})
export class MenuComponent {
    showMenu = false;
    constructor(private http: HttpClient, private router: Router){}
    
    show() {
        this.showMenu = false;
    }
    switch() {
        this.showMenu = !this.showMenu;
    }

     logout(){
         this.http.post("api/Patient/logout", User)
             .subscribe(resp => {
                 console.log(resp);
                 this.router.navigate(['/login']);
             })
        
     }
    
}
