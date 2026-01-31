import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { OAuthService } from 'angular-oauth2-oidc';
import { ToasterService } from '@abp/ng.theme.shared';
import { EnvironmentService } from '@abp/ng.core';
import { Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class SignalRService {
    private hubConnection: signalR.HubConnection | undefined;
    public reminderReceived = new Subject<any>();

    constructor(
        private oAuthService: OAuthService,
        private toaster: ToasterService,
        private environmentService: EnvironmentService
    ) { }

    public startConnection = () => {
        if (!this.oAuthService.hasValidAccessToken()) {
            return;
        }

        // Request notification permission
        if ('Notification' in window && Notification.permission !== 'granted') {
            Notification.requestPermission();
        }

        const apiUrl = this.environmentService.getApiUrl('default');
        const token = this.oAuthService.getAccessToken();

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(apiUrl + '/signalr-hubs/link-vault', {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        this.hubConnection
            .start()
            .then(() => console.log('SignalR Connection started'))
            .catch(err => console.log('Error while starting SignalR connection: ' + err));

        this.addReminderListener();
    }

    public addReminderListener = () => {
        this.hubConnection.on('ReceiveReminder', (data) => {
            // Emit event for components
            this.reminderReceived.next(data);

            // System Notification (Native OS Style)
            if ('Notification' in window && Notification.permission === 'granted') {
                const notification = new Notification(data.title || 'Link Vault Reminder', {
                    body: data.message,
                    requireInteraction: true, // Key for "reminder" behavior
                    icon: '/assets/images/logo/icon-lite.svg' // Try to find a valid icon path or leave default
                });

                if (data.url) {
                    notification.onclick = (e) => {
                        e.preventDefault();
                        window.open(data.url, '_blank');
                    };
                }
            }

            // In-App Toast
            this.toaster.info(data.message, data.title, {
                life: 15000,
                sticky: true,
                closable: true,
                tapToDismiss: false // Prevent accidental dismissal
            });
        });
    }
}
