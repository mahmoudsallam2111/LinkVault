import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { AppNotificationService } from '../../../../proxy/notifications/app-notification.service';
import { AppNotificationDto } from '../../../../proxy/notifications/models';
import { SignalRService } from '../../../services/signalr.service';
import { LocalizationModule } from '@abp/ng.core';

@Component({
    selector: 'app-notification-bell',
    standalone: true,
    imports: [CommonModule, NgbDropdownModule, LocalizationModule],
    templateUrl: './notification-bell.component.html',
    styleUrls: ['./notification-bell.component.css']
})
export class NotificationBellComponent implements OnInit, OnDestroy {
    unreadCount = 0;
    notifications: AppNotificationDto[] = [];
    loading = false;

    constructor(
        private notificationService: AppNotificationService,
        private signalRService: SignalRService
    ) { }

    ngOnInit() {
        this.loadUnreadCount();
        this.signalRService.reminderReceived.subscribe((data) => {
            this.unreadCount++;
            // If list is open or loaded, prepend
            if (this.notifications.length > 0) {
                this.notifications.unshift({
                    id: data.NotificationId || '',
                    title: data.Title,
                    message: data.Message,
                    url: data.Url,
                    isRead: false,
                    icon: 'fas fa-bell',
                    creationTime: new Date().toISOString()
                } as any);
            }
        });
    }

    ngOnDestroy() {
        // Unsubscribe logic
    }

    loadUnreadCount() {
        this.notificationService.getUnreadCount().subscribe(count => {
            this.unreadCount = count;
        });
    }

    loadNotifications() {
        this.loading = true;
        this.notificationService.getList({ unreadOnly: false, maxResultCount: 10, skipCount: 0 }).subscribe({
            next: (res) => {
                this.notifications = res.items;
                this.loading = false;
            },
            error: () => {
                this.loading = false;
            }
        });
    }

    onDropdownOpen(isOpen: boolean) {
        if (isOpen) {
            this.loadNotifications();
        }
    }

    markAsRead(notification: AppNotificationDto, event: MouseEvent) {
        event.stopPropagation(); // prevent dropdown close if needed, or let it close
        if (!notification.isRead) {
            this.notificationService.markAsRead(notification.id).subscribe(() => {
                notification.isRead = true;
                this.unreadCount = Math.max(0, this.unreadCount - 1);
            });
        }

        if (notification.url) {
            window.open(notification.url, '_blank');
        }
    }

    markAllAsRead(event: MouseEvent) {
        event.stopPropagation();
        this.notificationService.markAllAsRead().subscribe(() => {
            this.notifications.forEach(n => n.isRead = true);
            this.unreadCount = 0;
        });
    }
}
