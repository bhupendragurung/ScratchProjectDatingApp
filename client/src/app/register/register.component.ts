import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

maxDate:Date=new Date();
@Output() cancelRegister =new EventEmitter();
registerForm:FormGroup =new FormGroup({});
validationErrors:string[] | undefined;
  constructor(private accountService:AccountService, private toastr:ToastrService
    ,private fb:FormBuilder,private router:Router) { }

  ngOnInit(): void {
    this.intializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
  }
  private getDateOnly(dob:string | undefined){
if(!dob) return;
let theDob=new Date(dob);
return new Date(theDob.setMinutes(theDob.getMinutes())-theDob.getTimezoneOffset())
.toISOString().slice(0,10);
  }
  register(){
const dob=this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
const values={...this.registerForm.value,dateOfBirth:dob};
   
    this.accountService.register(values).subscribe(
      {
        next:() => {
this.router.navigateByUrl('/members');
        }
        ,error:error=> {
          this.validationErrors=error;
        }
        
      }
    );
  }
  cancel(){
   this.cancelRegister.emit(false);
  }
intializeForm(){
  this.registerForm=this.fb.group({
    gender:['male'],
    username:['',Validators.required],
    knownAs:['',Validators.required],
    dateOfBirth:['',Validators.required],
    city:['',Validators.required],
    country:['',Validators.required],
    password:['',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
    confirmPassword:['',[Validators.required,this.matchValue('password')]],

  });
  this.registerForm.controls['password'].valueChanges.subscribe({
    next:()=>{
      this.registerForm.controls['confirmPassword'].updateValueAndValidity();
    }
  })
}
matchValue(matchTo:string):ValidatorFn{
  return (control:AbstractControl)=>{
    return control.value === control.parent?.get(matchTo)?.value ?null:{notMatching:true};
  }
}
}
