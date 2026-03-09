import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';


// singleton, when injected it's a single instance per application 
@Injectable({
  providedIn: 'root'
})

// requests api/account
export class AccountService {
  baseUrl = "https://localhost:5001/api/"

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model)
  }
}
