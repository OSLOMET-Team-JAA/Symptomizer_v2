import { Component } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormArray, Validators, FormBuilder, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { Patient } from "../Patient";

export interface Symptoms {
    id: number;
    name: string;
}

@Component({
    templateUrl: 'add.component.html'
})
export class AddComponent {
    dataForm: FormGroup;
    symptomsForm: FormGroup;

    allSymptoms = [];

    symptomsList: Symptoms[] = [
        { id: 0, name: 'Fever or chills' },
        { id: 1, name: 'Cough' },
        { id: 2, name: 'Sore throat' },
        { id: 3, name: 'High temperature' },
        { id: 4, name: 'Shortness of breath or difficulty breathing' },
        { id: 5, name: 'Muscle or body aches' },
    ]

    validation = {
        id: [""],
        firstname: [
            null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,30}")])
        ],
        lastname: [
            null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,30}")])
        ]
    }

    constructor(private http: HttpClient, private fb: FormBuilder, private router: Router) {
        this.dataForm = fb.group(this.validation);
        this.symptomsForm = this.fb.group({
            isArray: this.fb.array([], [Validators.required])
        });
    }

    onChange(name: string, isChecked: boolean) {
        const symptoms = (this.symptomsForm.controls.name as FormArray);

        if (isChecked) {
            symptoms.push(new FormControl(name));
            this.allSymptoms.push(name)
        } else {
            const index = symptoms.controls.findIndex(x => x.value === name);
            symptoms.removeAt(index);
            this.allSymptoms.splice(index);
        }
        console.log(symptoms)
    }

    findDisease(symptoms) {
        const flu = ["Fever or chills", "Cough", "Sore throat", "High temperature", "Muscle or body aches"]
        const covid_19 = ["Fever or chills", "Cough", "Sore throat", "High temperature", "Shortness of breath or difficulty breathing", "Muscle or body aches"]
        let str1 = symptoms.sort().toString();
        let str2 = flu.sort().toString();
        let str3 = covid_19.sort().toString();
        if (str1.toLowerCase() === str2.toLowerCase()) {
            return "Flu";
        } else if (str1.toLowerCase() === str3.toLowerCase()) {
            return "COVID-19";
        } else {
            return "Common cold";
        }
    }

    ngOnInit() {
        this.symptomsForm = this.fb.group({
            name: this.fb.array([])
        });
    }

    onSubmit() {
        this.addPatient();
    }

    addPatient() {
        const addPatient = new Patient();
        addPatient.firstname = this.dataForm.value.firstname;
        addPatient.lastname = this.dataForm.value.lastname;
        addPatient.symptoms = this.allSymptoms.toString();
        addPatient.disease = this.findDisease(this.allSymptoms);
        // allSymptoms to be converted to string before usage ???????????
        console.log(addPatient)
        console.log(typeof (addPatient))

        //There was some troubles with json (default) return, so responseType was changed to 'text'
        this.http.post("api/Patient", addPatient, { responseType: 'text' })
            .subscribe(result => {
                console.log(result)
                this.router.navigate(['/home']);
            },
                error => console.log(error)
            );
    };
}
// https://angular.io/guide/http#requesting-non-json-data
