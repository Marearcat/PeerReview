import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from "@angular/router";

@Component({
  selector: 'app-account-register',
  templateUrl: './account.register.component.html'
})

export class AccountRegisterComponent {
  http: HttpClient;
  complete: boolean;
  specs: string[];

  constructor(http: HttpClient, router: Router) {
    this.http = http;
    this.complete = false;
    this.getPage();
  }

  getPage() {
    this.http.get<string[]>('https://localhost:44384/api/account/register').subscribe(result => { this.specs = result });
  }

  register(form: Registration) {
    const body = { key: form.key, fullname: form.fullname, nick: form.nick, password: form.password, categories: form.categories };
    this.http.post<boolean>('https://localhost:44384/api/account/register', body).subscribe(result => { this.complete = result });
  }

}

interface Registration {
  key: string;
  fullname: string;
  nick: string;
  password: string;
  categories: string[];
}
