import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AccountRegisterComponent } from './register/account.register.component';

@NgModule({
  declarations: [
    AccountRegisterComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: 'register', component: AccountRegisterComponent }
    ])
  ],
  providers: []
})
export class AccountModule { }
