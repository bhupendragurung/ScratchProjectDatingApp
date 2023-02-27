import { Injectable } from '@angular/core';
import {  CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<MemberEditComponent> {
  canDeactivate(
    component: MemberEditComponent): boolean {
      if(component.editForm?.dirty){
       return confirm('Are you sure want to continue.Any unsaved changes will be lost.')
      }
    return true;





  }
  
}
