
import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Patient } from "../Patient";


@Component({
    templateUrl: "edit.component.html"
})

export class EditComponent implements OnInit {
    dataForm: FormGroup;
    checkedSymptoms = []
    allSymptoms = []

    validation = {
        id: [""],
        firstname: [
            null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,30}")])
        ],
        lastname: [
            null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,30}")])
        ]
    }

    constructor(private http: HttpClient,
        private route: ActivatedRoute, private fb: FormBuilder, private router: Router) {
        this.dataForm = fb.group(this.validation);
        
    }

    // symptoms = (this.symptomsForm.controls.name as FormArray);
    // onChange(name: string, isChecked: boolean) {
    //     const symptomsNameList = (this.symptomsForm.controls.name as FormArray);
    //
    //     if (isChecked) {
    //         symptomsNameList.push(new FormControl(name));
    //         this.allSymptoms.push(name)
    //     } else {
    //         const index = symptomsNameList.controls.findIndex(x => x.value === name);
    //         symptomsNameList.removeAt(index);
    //         this.allSymptoms.splice(index);
    //     }
    //     console.log(symptomsNameList)
    // }

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
         });        
    }

    onSubmit() {
        this.checkCheckBoxes();        
        this.patientToEdit();
    }
    
    editPatient(id: number) {
        this.http.get<Patient>("api/Patient/" + id)
            .subscribe(
                p => {
                    document.getElementById("id").setAttribute('value', ""+p.id);                    
                    document.getElementById("firstname").setAttribute('value', ""+p.firstname);
                    document.getElementById("lastname").setAttribute('value', ""+p.lastname);
                    this.checkedSymptoms = (p.symptoms).split(',')
                    console.log("After split")
                    console.log(this.checkedSymptoms)                   
                    
                    // Collecting all selectors with symptoms ids
                    const nodeList = document.querySelectorAll('[id^="symptom"]') as NodeListOf<HTMLInputElement> ;                    
                    // Loop through all nodes in nodeList
                    nodeList.forEach(symptom => {
                        // comparing array values of checkedSymptoms and values from nodeList
                        for(let i=0; i< this.checkedSymptoms.length;i++){
                            if(this.checkedSymptoms[i] == symptom.value){
                                symptom.checked = true;
                            }
                        }
                    });
                },
                error => console.log(error)
            );
    }

    checkCheckBoxes(){
        // Collecting all selectors with symptoms ids
        const chBox = document.querySelectorAll('[id^="symptom"]') as NodeListOf<HTMLInputElement>;
        // Loop through all nodes in nodeList
        chBox.forEach(symptom => {
            if(symptom.checked){
                this.allSymptoms.push(symptom.value);
            }            
        });
        console.log(this.allSymptoms)
        console.log(this.allSymptoms.toString())
    }
    
       
    patientToEdit() {
        const id: number = 0;
        const firstname: string = "";
        const lastname: string = "";
        const readData = [];
        const readCheckBoxes = document.querySelectorAll('[id="id"], [id="firstname"], [id="lastname"]') as NodeListOf<HTMLInputElement>;
        readCheckBoxes.forEach(r => {
            if(readCheckBoxes != null){
                readData.push(r.value);
            }
        })
        console.log(readCheckBoxes)
        
        // const ePatient2 = {
        //     id: readData[0],
        //     firstname: readData[1],
        //     lastname: readData[2],
        //     symptoms: this.allSymptoms.toString(),
        //     disease: this.findDisease(this.allSymptoms)
        // }
        
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

// https://developer.mozilla.org/en-US/docs/Web/API/NodeList