import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from "@angular/router";
import { ResponseType } from '@angular/http';
import { text } from '@angular/core/src/render3/instructions';

@Component({
  selector: 'app-article-get',
  templateUrl: './article.get.component.html'
})

export class ArticleGetComponent {
  value: string;
  http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.http.get<string>('https://localhost:44384/api/article?id=kek', { responseType: 'text' as 'json' }).subscribe(result => {
      this.value = result;
    }, error => console.error(error));
  }


}
