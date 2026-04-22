import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { Message } from '../_models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { Group } from '../_models/group';
import { BusyService } from './busy.service';

@Injectable({
  providedIn: 'root'
})

export class MessageService {
  baseUrl = environment.apiUrl
  hubUrl = environment.hubUrl;
  private hubConn: HubConnection;
  private msgThreadSrc = new BehaviorSubject<Message[]>([]);
  msgThread$ = this.msgThreadSrc.asObservable();

  constructor(private http: HttpClient, private busyService: BusyService) { }

  createHubConnection(user: User, otherUser: string) {
    this.busyService.busy();
    this.hubConn = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUser, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConn.start().catch(err => console.log(err)).finally(() => this.busyService.idle());

    this.hubConn.on('ReceiveMessageThread', msg => {
      this.msgThreadSrc.next(msg)
    })

    this.hubConn.on('NewMessage', msg => {
      this.msgThread$.pipe(take(1)).subscribe(messages => {
        this.msgThreadSrc.next([...messages, msg]); // without mutating the state
      })
    })

    this.hubConn.on('UpdatedGroup', (group: Group) => {
      if(group.connections.some(x => x.username === otherUser)) {
        this.msgThread$.pipe(take(1)).subscribe(messages => {
          messages.forEach(msg => {
            if(!msg.dateRead) {
              msg.dateRead = new Date(Date.now());
            }
          })
          this.msgThreadSrc.next([...messages])
        })
      }
    })
  }

  stopHubConnection() {
    if(this.hubConn) {
      this.msgThreadSrc.next([]);
      this.hubConn?.stop();
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  async sendMessage(username: string, content: string) {
    return this.hubConn?.invoke('SendMessage', {recipientUsername: username, content}) // MessageHub.SendMessage(dto)
      .catch(err => console.log(err));
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id)
  }
}
