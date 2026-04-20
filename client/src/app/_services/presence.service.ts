import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private onlineUsersSrc = new BehaviorSubject<string[]>([]); // realtime event emitting
  onlineUsers$ = this.onlineUsersSrc.asObservable();

  constructor(private toastr: ToastrService) { }

  createHubConnection(user: User) { // auto connect users to the hub when authenticated -> [login / reg]
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build()

    this.hubConnection
      .start()
      .catch(err => console.log(err))

    this.hubConnection.on('UserIsOnline', username => {
      this.toastr.info(username + ' has connected')
    })

    this.hubConnection.on('UserIsOffline', username => {
      this.toastr.warning(username + ' has disconnected');
    })

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
      this.onlineUsersSrc.next(usernames);
    })
  }

  stopHubConnection() { // when logout
    this.hubConnection?.stop().catch(err => console.log(err))
  }
}
