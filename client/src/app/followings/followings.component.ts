import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-followings',
  templateUrl: './followings.component.html',
  styleUrls: ['./followings.component.css']
})
export class FollowingsComponent implements OnInit {
  members: Partial<Member[]> | undefined;
  predicate = 'followed';

  constructor(private membersService: MembersService) { }

  ngOnInit(): void {
    this.loadFollowings()
  }

  loadFollowings() {
    this.membersService.getFollowings(this.predicate).subscribe(response => {
      this.members = response;
    })
  }

}
