import { Component, OnInit } from '@angular/core';
import { Pagination } from '../_models/pagination';
import { MessageService } from '../_services/message.service';
import { Message } from '../_models/message';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[] = [];
  pagination: Pagination | undefined;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 6;
  loading = false;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.loading = true;
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(resp => {
      this.messages = resp.result;
      this.pagination = resp.pagination;
      this.loading = false;
    })
  }

  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe(() => {
      this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
    })
  }

  pageChanged(ev: any) {
    if(this.pageNumber !== ev.page) {
      this.pageNumber = ev.page;
      this.loadMessages();
    }
  }

}
