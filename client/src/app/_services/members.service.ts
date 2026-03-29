import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class MembersService { // can work as a state store since it's singleton, instead of using redux
  private baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private httpClient: HttpClient) { }

  getMembers() {
    if(this.members.length > 0) {
      return of(this.members);
    }
    return this.httpClient.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;
        return members;
      })
    )
  }

  getMember(username: string) {
    const member = this.members.find(mem => mem.username === username);
    if(member !== undefined) return of(member);
    return this.httpClient.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.httpClient.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const idx = this.members.indexOf(member);
        this.members[idx] = member;
      })
    )
  }
}
