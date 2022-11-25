
import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Patient } from "../Patient";

export interface Symptoms {
    id: number;
    name: string;
}

@Component({
    templateUrl: "edit.component.html"
})
export class EditComponent implements OnInit {
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

    constructor(private http: HttpClient, private fb: FormBuilder,
        private route: ActivatedRoute, private router: Router) {
        this.dataForm = fb.group(this.validation);

    }

    // symptoms = (this.symptomsForm.controls.name as FormArray);
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
        // this.route.params.subscribe(params => {
        //   this.editPatient(params.id);
        // });
        this.symptomsForm = this.fb.group({
            isArray: this.fb.array([], Validators.required)
        });
    }

    onSubmit() {
        this.patientToEdit();
    }

    editPatient(id: number) {
        this.http.get<Patient>("api/Patient/" + id)
            .subscribe(
                p => {
                    this.dataForm.patchValue({ id: p.id });
                    this.dataForm.patchValue({ firstname: p.firstname });
                    this.dataForm.patchValue({ lastname: p.lastname });
                    this.dataForm.patchValue({ symptoms: p.symptoms });
                    console.log(JSON.stringify(p.symptoms));
                    const recSymptoms = JSON.stringify(p.symptoms);

                    this.dataForm.patchValue({ disease: p.disease });

                },
                error => console.log(error)
            );
    }

    patientToEdit() {
        const ePatient = new Patient();
        ePatient.id = this.dataForm.value.id;
        ePatient.firstname = this.dataForm.value.firstname;
        ePatient.lastname = this.dataForm.value.lastname;
        ePatient.symptoms = this.allSymptoms.toString();
        ePatient.disease = this.findDisease(this.allSymptoms);

        this.http.put("api/Patient/", ePatient, { responseType: 'text' })
            .subscribe(
                result => {
                    console.log(result)
                    this.router.navigate(['/home']);
                },
                error => console.log(error)
            );
    }
}

