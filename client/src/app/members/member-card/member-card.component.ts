import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member | undefined;

  constructor(private membersService: MembersService, private toastr: ToastrService, public presence: PresenceService) { }

  ngOnInit(): void {
  }

  addFollow(member: Member) {
    this.membersService.addFollow(member.username).subscribe(() => {
      this.toastr.success('You have followed ' + member.knownAs);
    })
  }

  removeFollow(member: Member) {
    this.membersService.removeFollow(member.username).subscribe(() => {
      this.toastr.success('You have unfollowed ' + this.member?.knownAs)
    })
  }
}
