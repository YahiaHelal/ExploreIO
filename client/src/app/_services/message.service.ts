import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { Message } from '../_models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl
  hubUrl = environment.hubUrl;
  private hubConn?: HubConnection;
  private msgThreadSrc = new BehaviorSubject<Message[]>([]);
  msgThread$ = this.msgThreadSrc.asObservable();

  constructor(private http: HttpClient) { }

  createHubConnection(user: User, otherUser: string) {
    this.hubConn = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUser, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConn.start().catch(err => console.log(err));

    this.hubConn.on('ReceiveMessageThread', msg => {
      this.msgThreadSrc.next(msg)
    })

    this.hubConn.on('NewMessage', msg => {
      this.msgThread$.pipe(take(1)).subscribe(messages => {
        this.msgThreadSrc.next([...messages, msg]); // without mutating the state
      })
    })
  }

  stopHubConnection() {
    if(this.hubConn) {
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
