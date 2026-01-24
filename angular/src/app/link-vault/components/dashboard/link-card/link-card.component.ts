import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { LinkDto } from 'src/app/proxy/links/dtos';

@Component({
  selector: 'app-link-card',
  standalone: true,
  imports: [CommonModule, NgbTooltipModule],
  templateUrl: './link-card.component.html',
  styleUrls: ['./link-card.component.css'],
})
export class LinkCardComponent {
  @Input() link!: LinkDto;
  @Input() trashMode = false;
  @Output() visit = new EventEmitter<LinkDto>();
  @Output() favorite = new EventEmitter<LinkDto>();
  @Output() edit = new EventEmitter<LinkDto>();
  @Output() delete = new EventEmitter<LinkDto>();
  @Output() restore = new EventEmitter<LinkDto>();
  @Output() hardDelete = new EventEmitter<LinkDto>();

  get faviconUrl(): string {
    return this.link.favicon || 'assets/images/default-favicon.png'; // Fallback
  }

  onVisit() {
    this.visit.emit(this.link);
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
