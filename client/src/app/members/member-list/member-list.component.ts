import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { UserParams } from 'src/app/_models/userParams';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;

  constructor(private membersService: MembersService) {
    this.userParams = this.membersService.getUserParams();
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    if(this.userParams) {
      this.membersService.setUserParams(this.userParams);
      this.membersService.getMembers(this.userParams).subscribe({
        next: resp => {
        if(resp.result && resp.pagination) {
          this.members = resp.result;
          this.pagination = resp.pagination;
        }
      }})
    }
  }

  pageChanged(event: any) {
    if(this.userParams && this.userParams?.pageNumber !== event.page) {
      this.membersService.setUserParams(this.userParams);
      this.userParams.pageNumber = event.page;
      this.loadMembers()
    }
  }


}
