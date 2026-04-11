import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';


@Injectable({
  providedIn: 'root'
})
export class MembersService { // can work as a state store since it's singleton, instead of using redux
  private baseUrl = environment.apiUrl;
  members: Member[] = [];
  userParams: UserParams | undefined;
  membersCache = new Map();

  constructor(private httpClient: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(()=> {
      this.userParams = new UserParams();
    })
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(userParams: UserParams) {
    this.userParams = userParams;
  }
  resetUserParams() {
    this.userParams = new UserParams();
    return this.userParams;
  }

  getMembers(userParams: UserParams) {
    var val = Object.values(userParams).join('-');
    var resp = this.membersCache.get(val);
    if(resp) {
      return of(resp);
    }
    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    params = params.append('orderBy', userParams.orderBy);
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params).pipe(
      map(resp => {
        this.membersCache.set(val, resp);
        return resp;
      })
    )
  }

  getMember(username: string) {
    const member = [...this.membersCache.values()]
      .reduce((prev, curr) => prev.concat(curr.result), [])
      .find((mem: Member) => mem.username === username)
    if(member) return of(member);

    return this.httpClient.get<Member>(this.baseUrl + 'users/' + username)
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

  addFollow(username: string) {
    return this.httpClient.post(this.baseUrl + 'followings/' + username, {});
  }

  removeFollow(username: string) {
    return this.httpClient.delete(this.baseUrl + 'followings/' + username);
  }

  getFollowings(predicate: string, pageNumber: number, pageSize: number) {
    let params = this.getPaginationHeaders(pageNumber,pageSize);
    params = params.append('predicate', predicate);
    return this.getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'followings', params);
  }

  private getPaginatedResult<T>(url, params) {
    const paginatedResult: PaginatedResult<T>  = new PaginatedResult<T>();

    return this.httpClient.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());
    return params;
  }
}
