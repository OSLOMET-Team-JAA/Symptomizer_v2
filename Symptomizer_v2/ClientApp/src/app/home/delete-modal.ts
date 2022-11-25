import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    templateUrl: 'delete-modal.html'
})
export class Modal {
    constructor(public modal: NgbActiveModal) { }
}
