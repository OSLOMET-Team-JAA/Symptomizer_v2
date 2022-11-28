import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddComponent } from './add/add.component';
import { HomeComponent } from './home/home.component';
import { EditComponent } from './edit/edit.component';
import { LoginComponent } from "./login/login.component";


const appRoots: Routes = [    
    { path: 'login', component: LoginComponent},
    { path: 'home', component: HomeComponent },
    { path: 'add', component: AddComponent },
    { path: 'edit/:id', component: EditComponent },
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    //redirect to home
    { path: '**', redirectTo:''},   
]

@NgModule({
    imports: [
        RouterModule.forRoot(appRoots)
    ],
    exports: [
        RouterModule
    ]
})
export class AppRoutingModule { }
