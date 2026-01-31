import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  NgbModal,
  NgbTooltipModule,
  NgbDropdownModule,
  NgbOffcanvas,
} from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { LocalizationModule } from '@abp/ng.core';
import { LinkReminderService, LinkReminderDto } from 'src/app/proxy/reminders';
import { PagedResultDto } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
  selector: 'app-reminders',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    NgbTooltipModule,
    NgbDropdownModule,
    NgxDatatableModule,
    LocalizationModule,
    SidebarComponent,
  ],
  templateUrl: './reminders.component.html',
  styleUrls: ['./reminders.component.css'],
})
export class RemindersComponent implements OnInit {
  reminders: LinkReminderDto[] = [];
  filteredReminders: LinkReminderDto[] = [];
  totalCount = 0;
  isLoading = false;
  page = 1;
  pageSize = 20;
  searchTerm = '';

  constructor(
    private reminderService: LinkReminderService,
    private toaster: ToasterService,
    private offcanvasService: NgbOffcanvas,
  ) {}

  ngOnInit(): void {
    this.loadReminders();
  }

  loadReminders(): void {
    this.isLoading = true;
    this.reminderService
      .getList({
        skipCount: (this.page - 1) * this.pageSize,
        maxResultCount: this.pageSize,
        sorting: 'remindAt asc',
      })
      .subscribe({
        next: (result: PagedResultDto<LinkReminderDto>) => {
          this.reminders = result.items || [];
          this.filteredReminders = [...this.reminders];
          this.totalCount = result.totalCount || 0;
          this.isLoading = false;
          if (this.searchTerm) {
            this.onSearch(this.searchTerm);
          }
        },
        error: err => {
          console.error('Error loading reminders:', err);
          this.isLoading = false;
        },
      });
  }

  onSearch(term: string): void {
    this.searchTerm = term;
    if (!term) {
      this.filteredReminders = [...this.reminders];
      return;
    }
    const lowerTerm = term.toLowerCase();
    this.filteredReminders = this.reminders.filter(
      r =>
        r.linkTitle?.toLowerCase().includes(lowerTerm) ||
        r.note?.toLowerCase().includes(lowerTerm) ||
        r.linkUrl?.toLowerCase().includes(lowerTerm),
    );
  }

  openMobileSidebar() {
    this.offcanvasService.open(SidebarComponent, {
      position: 'start',
      panelClass: 'sidebar-offcanvas',
    });
  }

  deleteReminder(reminder: LinkReminderDto): void {
    if (!confirm('Are you sure you want to cancel this reminder?')) {
      return;
    }

    this.reminderService.delete(reminder.id!).subscribe({
      next: () => {
        this.toaster.success('Reminder cancelled');
        this.loadReminders();
      },
      error: err => {
        console.error('Error deleting reminder:', err);
        this.toaster.error('Failed to cancel reminder');
      },
    });
  }

  openLink(reminder: LinkReminderDto): void {
    if (reminder.linkUrl) {
      window.open(reminder.linkUrl, '_blank');
    }
  }

  getTimeRemaining(remindAt: string): string {
    const now = new Date();
    const target = new Date(remindAt);
    const diff = target.getTime() - now.getTime();

    if (diff <= 0) {
      return 'Due now';
    }

    const minutes = Math.floor(diff / (1000 * 60));
    const hours = Math.floor(diff / (1000 * 60 * 60));
    const days = Math.floor(diff / (1000 * 60 * 60 * 24));

    if (days > 0) {
      return `${days} day${days > 1 ? 's' : ''}`;
    } else if (hours > 0) {
      return `${hours} hour${hours > 1 ? 's' : ''}`;
    } else {
      return `${minutes} minute${minutes > 1 ? 's' : ''}`;
    }
  }

  getColorForDomain(domain: string): string {
    const colors = ['#696cff', '#71dd37', '#03c3ec', '#ff3e1d', '#ffab00', '#233446'];
    let hash = 0;
    for (let i = 0; i < domain.length; i++) {
      hash = domain.charCodeAt(i) + ((hash << 5) - hash);
    }
    const index = Math.abs(hash % colors.length);
    return colors[index];
  }
}
