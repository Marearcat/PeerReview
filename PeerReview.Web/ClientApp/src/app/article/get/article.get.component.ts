import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-article-get',
  templateUrl: './article.get.component.html'
})

export class ArticleGetComponent {
  value: string;
  http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.http.get<string>('https://localhost:44384/api/article?id=kek').subscribe(result => {
      this.value = result;
    }, error => console.error(error));
  }


}
