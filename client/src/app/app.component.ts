import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ɵɵsetComponentScope } from '@angular/core';

interface User{

  id: number;
  userName: string
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'client';
  users!: User[];

  constructor(private http: HttpClient) {
    // this.users = '';
  }

  ngOnInit() {


    this.http.get<User[]>('https://localhost:5001/api/users').subscribe(
      (res) => {

        this.users = res;

        console.log(this.users)
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
