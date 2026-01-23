import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { NgbModal, NgbDropdownModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { LocalizationModule, LocalizationService } from '@abp/ng.core';
import { LinkModalComponent } from '../dashboard/link-modal/link-modal.component';
import { ConfirmationService, ToasterService } from '@abp/ng.theme.shared';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { LinkService } from '../../../proxy/links/link.service';
import { CollectionService } from '../../../proxy/collections/collection.service';
import { LinkDto, LinkFilterDto } from '../../../proxy/links/models';
import { CollectionDto } from '../../../proxy/collections/models';

@Component({
  selector: 'app-collection-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    NgxDatatableModule,
    NgbDropdownModule,
    SidebarComponent,
    LocalizationModule,
  ],
  templateUrl: './collection-detail.component.html',
  styleUrls: ['./collection-detail.component.css'],
})
export class CollectionDetailComponent implements OnInit {
  collection: CollectionDto | null = null;
  links: LinkDto[] = [];
  loading = false;
  searchTerm = '';
  collectionId: string = '';
  isSharing = false;

  filter: LinkFilterDto = {
    skipCount: 0,
    maxResultCount: 100,
    includeDeleted: false,
  };

  // Sort options
  sortOptions = [
    { value: 'creationTime desc', labelKey: 'LinkVault::NewestFirst' },
    { value: 'creationTime asc', labelKey: 'LinkVault::OldestFirst' },
    { value: 'title asc', labelKey: 'LinkVault::TitleAZ' },
    { value: 'title desc', labelKey: 'LinkVault::TitleZA' },
    { value: 'visitCount desc', labelKey: 'LinkVault::MostVisited' },
  ];
  selectedSort = 'creationTime desc';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private linkService: LinkService,
    private collectionService: CollectionService,
    private modalService: NgbModal,
    private offcanvasService: NgbOffcanvas,
    private confirmation: ConfirmationService,
    private toaster: ToasterService,
    public localization: LocalizationService,
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.collectionId = params['id'];
      if (this.collectionId) {
        this.filter.collectionId = this.collectionId;
        this.loadCollection();
        this.loadLinks();
      }
    });
  }

  loadCollection() {
    this.collectionService.get(this.collectionId).subscribe({
      next: res => {
        this.collection = res;
      },
      error: err => {
        console.error('Failed to load collection', err);
        // Navigate back to dashboard if collection not found
        this.router.navigate(['/']);
      },
    });
  }

  loadLinks() {
    this.loading = true;
    this.filter.sorting = this.selectedSort;
    this.linkService.getList(this.filter).subscribe({
      next: res => {
        this.links = res.items;
        this.loading = false;
      },
      error: err => {
        console.error('Failed to load links', err);
        this.loading = false;
      },
    });
  }

  onSearch(event: Event) {
    const target = event.target as HTMLInputElement;
    this.searchTerm = target.value;
    this.filter.filter = this.searchTerm;
    this.loadLinks();
  }

  onSortChange() {
    this.filter.sorting = this.selectedSort;
    this.loadLinks();
  }

  goBack() {
    this.router.navigate(['/']);
  }

  // Link Actions
  addLink() {
    const modalRef = this.modalService.open(LinkModalComponent, {
      size: 'lg',
      centered: true,
      backdrop: 'static',
    });
    modalRef.componentInstance.mode = 'create';
    // Pre-select this collection when adding new link
    modalRef.componentInstance.preselectedCollectionId = this.collectionId;

    modalRef.result.then(
      result => {
        if (result) {
          this.loadLinks();
          this.loadCollection(); // Refresh link count
        }
      },
      () => {
        /* Modal dismissed */
      },
    );
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
          // If collection changed, remove from list
          if (result.collectionId !== this.collectionId) {
            this.links = this.links.filter(l => l.id !== link.id);
            this.loadCollection(); // Refresh link count
          } else {
            this.loadLinks();
          }
        }
      },
      () => {
        /* Modal dismissed */
      },
    );
  }

  deleteLink(link: LinkDto) {
    this.confirmation
      .warn(`Are you sure you want to delete "${link.title}"?`, 'Delete Link')
      .subscribe(status => {
        if (status === 'confirm') {
          this.linkService.delete(link.id).subscribe({
            next: () => {
              this.links = this.links.filter(l => l.id !== link.id);
              this.loadCollection(); // Refresh link count
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

  toggleFavorite(link: LinkDto) {
    this.linkService.toggleFavorite(link.id).subscribe({
      next: updatedLink => {
        link.isFavorite = updatedLink.isFavorite;
      },
      error: err => {
        console.error('Failed to toggle favorite', err);
      },
    });
  }

  visitLink(link: LinkDto) {
    // Increment the local visit count immediately for UI feedback
    link.visitCount = (link.visitCount || 0) + 1;
    this.linkService.incrementVisit(link.id).subscribe({
      next: updatedLink => {
        link.visitCount = updatedLink.visitCount;
      },
    });
    window.open(link.url, '_blank');
  }

  openMobileSidebar() {
    this.offcanvasService.open(SidebarComponent, {
      position: 'start',
      panelClass: 'sidebar-offcanvas',
    });
  }

  shareCollection() {
    if (!this.collection) return;

    this.isSharing = true;

    // If already has a share token, use it; otherwise generate one
    if (this.collection.publicShareToken) {
      this.openWhatsAppShare(this.collection.publicShareToken);
      this.isSharing = false;
    } else {
      this.collectionService.generateShareToken(this.collectionId).subscribe({
        next: updatedCollection => {
          this.collection = updatedCollection;
          if (updatedCollection.publicShareToken) {
            this.openWhatsAppShare(updatedCollection.publicShareToken);
          }
          this.isSharing = false;
        },
        error: err => {
          console.error('Failed to generate share token', err);
          this.toaster.error('Failed to generate share link');
          this.isSharing = false;
        },
      });
    }
  }

  revokeShare() {
    if (!this.collection) return;

    this.confirmation
      .warn('Are you sure you want to revoke the share link? Anyone with the link will no longer be able to access this collection.', 'Revoke Share')
      .subscribe(status => {
        if (status === 'confirm') {
          this.collectionService.revokeShareToken(this.collectionId).subscribe({
            next: () => {
              if (this.collection) {
                this.collection.publicShareToken = undefined;
              }
              this.toaster.success('Share link revoked successfully');
            },
            error: err => {
              console.error('Failed to revoke share token', err);
              this.toaster.error('Failed to revoke share link');
            },
          });
        }
      });
  }

  private openWhatsAppShare(token: string) {
    const publicUrl = `${window.location.origin}/share/${token}`;
    const message = `Check out my collection "${this.collection?.name}": ${publicUrl}`;
    const whatsappUrl = `https://wa.me/?text=${encodeURIComponent(message)}`;
    window.open(whatsappUrl, '_blank');
  }
}
