import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal, NgbDropdownModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { LocalizationModule, LocalizationService } from '@abp/ng.core';
import { LinkCardComponent } from '../dashboard/link-card/link-card.component';
import { LinkModalComponent } from '../dashboard/link-modal/link-modal.component';
import { ConfirmationService, ToasterService } from '@abp/ng.theme.shared';
import { LinkService } from '../../../proxy/links/link.service';

import { SidebarComponent } from '../sidebar/sidebar.component';
import { ReminderModalComponent } from '../reminder-modal/reminder-modal.component';
import { LinkDto, LinkFilterDto } from 'src/app/proxy/links/dtos';

@Component({
  selector: 'app-favorites',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NgxDatatableModule,
    NgbDropdownModule,
    SidebarComponent,
    LocalizationModule,
  ],
  templateUrl: './favorites.component.html',
  styleUrls: ['./favorites.component.css'],
})
export class FavoritesComponent implements OnInit {
  links: LinkDto[] = [];
  loading = false;
  searchTerm = '';

  filter: LinkFilterDto = {
    skipCount: 0,
    maxResultCount: 100,
    includeDeleted: false,
    isFavorite: true,
  };

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
    this.linkService.getList(this.filter).subscribe({
      next: res => {
        this.links = res.items;
        this.loading = false;
      },
      error: err => {
        console.error('Failed to load favorites', err);
        this.loading = false;
      },
    });
  }

  onSearch(term: string) {
    this.filter.filter = term;
    this.loadData();
  }

  toggleFavorite(link: LinkDto) {
    this.linkService.toggleFavorite(link.id).subscribe(() => {
      // Remove from list since it's no longer a favorite
      this.links = this.links.filter(l => l.id !== link.id);
    });
  }

  deleteLink(link: LinkDto) {
    this.confirmation
      .warn(`Are you sure you want to delete "${link.title}"?`, 'Delete Link')
      .subscribe(status => {
        if (status === 'confirm') {
          this.linkService.delete(link.id).subscribe({
            next: () => {
              this.links = this.links.filter(l => l.id !== link.id);
              this.toaster.success('Link deleted successfully');
            },
            error: err => {
              console.error('Failed to delete link', err);
              this.toaster.error('Failed to delete link');
            },
          });
        }
      });
  }

  editLink(link: LinkDto) {
    const modalRef = this.modalService.open(LinkModalComponent, {
      size: 'lg',
      centered: true,
      backdrop: 'static',
    });
    modalRef.componentInstance.mode = 'edit';
    modalRef.componentInstance.link = link;

    modalRef.result.then(
      result => {
        if (result) {
          // If favorite status changed in edit, remove from list ??
          // Result contains updated link. Check isFavorite.
          if (!result.isFavorite) {
            this.links = this.links.filter(l => l.id !== link.id);
          } else {
            this.loadData();
          }
        }
      },
      () => { },
    );
  }

  setReminder(link: LinkDto) {
    const modalRef = this.modalService.open(ReminderModalComponent, {
      centered: true,
      backdrop: 'static',
    });
    modalRef.componentInstance.link = link;

    modalRef.result.then(
      result => {
        if (result === 'success') {
          this.toaster.success('Reminder set successfully');
          this.loadData();
        }
      },
      () => { },
    );
  }

  visitLink(link: LinkDto) {
    // Increment the local visit count immediately for UI feedback
    link.visitCount = (link.visitCount || 0) + 1;
    this.linkService.incrementVisit(link.id).subscribe();
    window.open(link.url, '_blank');
  }

  openMobileSidebar() {
    this.offcanvasService.open(SidebarComponent, {
      position: 'start',
      panelClass: 'sidebar-offcanvas',
    });
  }
}
