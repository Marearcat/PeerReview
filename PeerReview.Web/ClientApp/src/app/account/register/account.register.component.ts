import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from "@angular/router";

@Component({
  selector: 'app-account-register',
  templateUrl: './account.register.html'
})

export class MarketComponent {


  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, router: Router) {

  }


}
