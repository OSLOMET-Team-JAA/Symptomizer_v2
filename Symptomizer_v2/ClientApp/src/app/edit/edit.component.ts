import { Component } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, Validators, FormBuilder, FormArray, FormControl } from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import { Patient } from "../Patient";

@Component({
    templateUrl: 'edit.component.html'
})

export class EditComponent {
    dataForm: FormGroup;
    symptomsForm: FormGroup;
    getSymptoms = [];
    allSymptoms = [];
    
    
    symptomsList: any = [
        { id: 1, name: 'Fever or chills'},
        { id: 2, name: 'Cough'},
        { id: 3, name: 'Sore throat' },
        { id: 4, name: 'High temperature'},
        { id: 5, name: 'Shortness of breath or difficulty breathing'},
        { id: 6, name: 'Muscle or body aches' },
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
        this.dataForm = fb.group(
            this.validation,
            );
        this.symptomsForm = this.fb.group({
            sForm: this.fb.array([], [Validators.required])
        });
    }

    onChange(isChecked) {
        let value = isChecked.target.value;
        let check = isChecked.target.checked;
        const sForm: FormArray = this.symptomsForm.get('sForm') as FormArray;

        if (check) {
            sForm.push(new FormControl(value));
            this.allSymptoms.push(value);
        } else {
            this.remove(value, check,sForm, this.allSymptoms);            
        }
    }
    
    remove(value, check, formArray, array){
        let i: number = 0;
        formArray.controls.forEach((item: FormControl) => {
            if (item.value == check.target.value) {
                formArray.removeAt(i);
                array.filter(item => item !== value)
                return;
            }
            i++;
        });
        
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
        this.route.params.subscribe(params => {            
            this.editPatient(params.id);
        })
    }

    onSubmit() {
        this.updatePatientInfo();
    }

    editPatient(id: number) {
        this.http.get<Patient>("api/Patient/" + id)
            .subscribe(
                p => {
                    this.dataForm.patchValue({ id: p.id });
                    this.dataForm.patchValue({ firstname: p.firstname });
                    this.dataForm.patchValue({ lastname: p.lastname });
                    
                    this.getSymptoms = (p.symptoms).split(',');         
                    console.log("recovered symptoms are: ")
                    console.log(this.getSymptoms)
                    console.log("type of array with symptoms is: ")
                    console.log(typeof(this.getSymptoms))

                    // Symptoms recovery procedure. Collecting all selectors with symptoms ids
                    // const nodeList = document.querySelectorAll('[id^="symptom"]') as NodeListOf<HTMLInputElement>;
                    // console.log("NodeList is. ")
                    // console.log(nodeList)
                    // // Loop through all nodes in nodeList
                    // nodeList.forEach(symptom => {
                    //     // comparing array values of checkedSymptoms and values from nodeList
                    //     for(let i=0; i< this.allSymptoms.length;i++){
                    //         if(this.allSymptoms[i] == symptom.value){
                    //             symptom.checked = true;
                    //         }                          
                    //     }                        
                    // });
                    // console.log(this.symptomsList)
                },
                error => console.log(error)
            );
    }

    
    updatePatientInfo(){
        console.log("allSymptoms list have: ")
        console.log(this.allSymptoms)
        const  updatedPatient = new Patient();
        updatedPatient.id = this.dataForm.value.id;
        updatedPatient.firstname = this.dataForm.value.firstname;
        updatedPatient.lastname = this.dataForm.value.lastname;
        updatedPatient.symptoms = this.allSymptoms.toString();
        updatedPatient.disease = this.findDisease(this.allSymptoms);

        this.http.put("api/Patient/", updatedPatient, {responseType: "text"})
            .subscribe(
                reply => {
                    console.log(reply)                    
                    this.router.navigate(['/home']);
                },
                error => console.log(error)
            );
    }
    
}
//---------------- REFERENCES -------------------------------//
// https://remotestack.io/angular-checkboxes-tutorial-example/