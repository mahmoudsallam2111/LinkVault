import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal, NgbDropdownModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { LocalizationModule, LocalizationService } from '@abp/ng.core';
import { ConfirmationService, ToasterService } from '@abp/ng.theme.shared';
import { LinkService } from '../../../proxy/links/link.service';
import { LinkDto } from '../../../proxy/links/models';

import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
  selector: 'app-trash',
  standalone: true,
  imports: [CommonModule, FormsModule, NgxDatatableModule, NgbDropdownModule, SidebarComponent, LocalizationModule],
  templateUrl: './trash.component.html',
  styleUrls: ['./trash.component.css'],
})
export class TrashComponent implements OnInit {
  links: LinkDto[] = [];
  loading = false;
  searchTerm = '';

  constructor(
    private linkService: LinkService,
    private modalService: NgbModal,
    private offcanvasService: NgbOffcanvas,
    private confirmation: ConfirmationService,
    private toaster: ToasterService,
    public localization: LocalizationService,
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    // getTrash takes PagedAndSortedResultRequestDto
    this.linkService
      .getTrash({
        skipCount: 0,
        maxResultCount: 100,
        sorting: 'deletionTime desc',
      })
      .subscribe({
        next: res => {
          this.links = res.items;
          this.loading = false;
        },
        error: err => {
          console.error('Failed to load trash', err);
          this.loading = false;
        },
      });
  }

  onSearch(term: string) {
    if (!term) {
      this.loadData();
      return;
    }
    this.links = this.links.filter(
      l =>
        l.title?.toLowerCase().includes(term.toLowerCase()) ||
        l.url?.toLowerCase().includes(term.toLowerCase()),
    );
  }

  restoreLink(link: LinkDto) {
    this.linkService.restore(link.id).subscribe({
      next: () => {
        this.links = this.links.filter(l => l.id !== link.id);
        this.toaster.success('Link restored successfully');
      },
      error: err => {
        console.error('Failed to restore link', err);
        this.toaster.error('Failed to restore link');
      },
    });
  }

  hardDeleteLink(link: LinkDto) {
    this.confirmation
      .warn(
        `Are you sure you want to permanently delete "${link.title}"? This cannot be undone.`,
        'Delete Forever',
      )
      .subscribe(status => {
        if (status === 'confirm') {
          this.linkService.hardDelete(link.id).subscribe({
            next: () => {
              this.links = this.links.filter(l => l.id !== link.id);
              this.toaster.success('Link permanently deleted');
            },
            error: err => {
              console.error('Failed to delete link permanently', err);
              this.toaster.error('Failed to delete link permanently');
            },
          });
        }
      });
  }

  openMobileSidebar() {
    this.offcanvasService.open(SidebarComponent, {
      position: 'start',
      panelClass: 'sidebar-offcanvas'
    });
  }
}
