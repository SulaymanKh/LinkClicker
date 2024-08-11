import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { MatDialog } from '@angular/material/dialog';
import { LinkCreationCompletedDialogComponent } from '../link-creation-completed-dialog/link-creation-completed-dialog.component';
import { ConfigService } from '../services/config.service'; 

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: HubConnection;

  constructor(
    private dialog: MatDialog,
    private configService: ConfigService
) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.configService.webApiUrl}linkCreationHub`, {
        withCredentials: true
      })
      .configureLogging(LogLevel.Information)
      .build();

    this.hubConnection.start().catch(err => console.error('Error starting SignalR connection: ', err));
  }

  onLinkCreationCompleted(callback: (requestId: string, links: any[]) => void) {
    this.hubConnection.on('LinkCreationCompleted', (requestId: string, links: any[]) => {
      this.openLinkCreationCompletedDialog();
      callback(requestId, links);
    });
  }

  private openLinkCreationCompletedDialog(): void {
    this.dialog.open(LinkCreationCompletedDialogComponent, {
      width: '300px',
      data: { position: 'bottom-left' },
      panelClass: 'no-backdrop',
      disableClose: true,
      hasBackdrop: false
    });
  }
}
