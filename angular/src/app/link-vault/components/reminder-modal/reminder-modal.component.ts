import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import {
  LinkReminderService,
  CreateLinkReminderDto,
  UserReminderSettingsDto,
} from 'src/app/proxy/reminders';
import { LinkDto } from 'src/app/proxy/links/dtos';

interface ReminderOption {
  label: string;
  hours: number;
}

@Component({
  selector: 'app-reminder-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbModalModule],
  templateUrl: './reminder-modal.component.html',
  styleUrls: ['./reminder-modal.component.css'],
})
export class ReminderModalComponent implements OnInit {
  @Input() link!: LinkDto;
  @Output() reminderSet = new EventEmitter<void>();

  presetOptions: ReminderOption[] = [
    { label: '1 hour', hours: 1 },
    { label: '3 hours', hours: 3 },
    { label: 'Tomorrow', hours: 24 },
    { label: '3 days', hours: 72 },
    { label: '1 week', hours: 168 },
  ];

  selectedPreset: number | null = null;
  useCustomTime = false;
  customDateTime: string = '';
  note: string = '';
  isLoading = false;
  errorMessage = '';

  constructor(
    public activeModal: NgbActiveModal,
    private reminderService: LinkReminderService,
  ) {}

  ngOnInit(): void {
    // Set default custom datetime to tomorrow
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    tomorrow.setHours(9, 0, 0, 0);
    this.customDateTime = this.formatDateTimeLocal(tomorrow);
  }

  formatDateTimeLocal(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  selectPreset(hours: number): void {
    this.selectedPreset = hours;
    this.useCustomTime = false;
  }

  enableCustomTime(): void {
    this.useCustomTime = true;
    this.selectedPreset = null;
  }

  async setReminder(): Promise<void> {
    this.errorMessage = '';
    this.isLoading = true;

    try {
      const input: CreateLinkReminderDto = {
        linkId: this.link.id!,
        note: this.note || undefined,
      };

      if (this.useCustomTime && this.customDateTime) {
        input.remindAt = new Date(this.customDateTime).toISOString();
      } else if (this.selectedPreset) {
        input.durationHours = this.selectedPreset;
      } else {
        this.errorMessage = 'Please select a reminder time';
        this.isLoading = false;
        return;
      }

      await this.reminderService.create(input).toPromise();
      this.reminderSet.emit();
      this.activeModal.close('success');
    } catch (error: any) {
      console.error('Error setting reminder:', error);
      this.errorMessage = error?.error?.error?.message || 'Failed to set reminder';
    } finally {
      this.isLoading = false;
    }
  }

  getNow(): Date {
    return new Date();
  }
}
