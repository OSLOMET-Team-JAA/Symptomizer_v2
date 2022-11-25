import { Component } from '@angular/core';

@Component({
    selector: 'app-menu',
    templateUrl: './menu.component.html'
})
export class MenuComponent {
    showMenu = false;
    show() {
        this.showMenu = false;
    }
    switch() {
        this.showMenu = !this.showMenu;
    }

}
