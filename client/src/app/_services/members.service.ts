import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { PaginatedResult } from '../_models/pagination';


@Injectable({
  providedIn: 'root'
})
export class MembersService { // can work as a state store since it's singleton, instead of using redux
  private baseUrl = environment.apiUrl;
  members: Member[] = [];
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  constructor(private httpClient: HttpClient) { }

  getMembers(page?: number, itemsPerPage?: number) {
    let params = new HttpParams();
    if(page !== null && itemsPerPage !== null) {
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());
    }

    return this.httpClient.get<Member[]>(this.baseUrl + 'users', {observe: 'response', params}).pipe(
      map(response => {
        this.paginatedResult.result = response.body;
        if(response.headers.get('Pagination') !== null) {
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return this.paginatedResult;
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

  setMainPhoto(photoId: number) {
    return this.httpClient.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.httpClient.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }
}
