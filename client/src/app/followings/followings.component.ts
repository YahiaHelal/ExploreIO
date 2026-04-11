import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-followings',
  templateUrl: './followings.component.html',
  styleUrls: ['./followings.component.css']
})
export class FollowingsComponent implements OnInit {
  members: Partial<Member[]> | undefined;
  predicate = 'followed';
  pageNumber = 1;
  pageSize = 6;
  pagination: Pagination | undefined;

  constructor(public membersService: MembersService) { }

  ngOnInit(): void {
    this.loadFollowings()
  }

  loadFollowings() {
    this.membersService.getFollowings(this.predicate, this.pageNumber, this.pageSize).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChanged(ev: any) {
    this.pageNumber = ev.page;
    this.loadFollowings();
  }

}
