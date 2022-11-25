import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddComponent } from './add/add.component';
import { HomeComponent } from './home/home.component';
import { EditComponent } from './edit/edit.component';


const appRoots: Routes = [
    { path: 'home', component: HomeComponent },
    { path: 'add', component: AddComponent },
    { path: 'edit/:id', component: EditComponent, },
    { path: '', redirectTo: 'home', pathMatch: 'full' }
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
