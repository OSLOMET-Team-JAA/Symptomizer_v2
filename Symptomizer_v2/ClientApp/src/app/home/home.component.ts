import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { Modal } from './delete-modal';
import { Patient } from "../Patient";

@Component({
    /*selector: 'app-home',*/
    templateUrl: 'home.component.html'
})
export class HomeComponent {

    allPatients: Array<Patient>;
    loader: boolean;
    delPatient: string;


    constructor(private http: HttpClient, private router: Router, private modalService: NgbModal) { }


    ngOnInit() {
        this.loader = true;
        this.findAllPatients();
    }

    findAllPatients() {
        this.http.get<Patient[]>("api/Patient/")
            .subscribe(patients => {
                this.allPatients = patients;
                this.loader = false;
            },
                error => console.log(error)
            );
    };

    deletePatient(id: number) {

        // først hent navnet på kunden
        this.http.get<Patient>("api/Patient/" + id)
            .subscribe(patient => {
                this.delPatient = patient.firstname + " " + patient.lastname;

                // så vis modalen og evt. kall til slett
                this.showModalDelete(id);
            },
                error => console.log(error)
            );
    };

    showModalDelete(id: number) {
        const modal = this.modalService.open(Modal);

        modal.componentInstance.firstname = this.delPatient;

        modal.result.then(result => {
            console.log('Closed with: ' + result);
            if (result == "Delete") {

                // kall til server for sletting
                this.http.delete("api/Patient/" + id, { responseType: 'text' })
                    .subscribe(result => {
                        console.log(result)
                        this.findAllPatients();
                    },
                        error => console.log(error)
                    );
            }
            this.router.navigate(['/home']);
        });
    }
}
