import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown'
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { ToastrModule } from 'ngx-toastr';
import {TabsModule} from 'ngx-bootstrap/tabs';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FileUploadModule } from 'ng2-file-upload';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { TimeagoModule } from 'ngx-timeago';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    NgxGalleryModule,
    BsDropdownModule.forRoot(),
     TabsModule.forRoot(),
    ToastrModule.forRoot({
      positionClass:'toast-bottom-right'
    }),
    NgxSpinnerModule.forRoot({ type: 'line-scale-party' }),
    FileUploadModule,
    BsDatepickerModule.forRoot(),
    PaginationModule.forRoot(),
    ButtonsModule.forRoot(),
    TimeagoModule.forRoot()
  ],
  exports:[
     BsDropdownModule,
    ToastrModule,
     TabsModule,
    NgxGalleryModule,
    NgxSpinnerModule,
    FileUploadModule,
     BsDatepickerModule,
     PaginationModule,
     ButtonsModule,
     TimeagoModule
  ]
})
export class SharedModule { }
