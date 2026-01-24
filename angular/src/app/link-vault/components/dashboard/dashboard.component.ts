import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  NgbModal,
  NgbDropdownModule,
  NgbDatepickerModule,
  NgbDateStruct,
  NgbOffcanvas,
} from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { LocalizationModule, LocalizationService } from '@abp/ng.core';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { StatsCardComponent } from './stats-card/stats-card.component';
import { LinkModalComponent } from './link-modal/link-modal.component';
import { ConfirmationService, ToasterService } from '@abp/ng.theme.shared';
import { LinkService } from '../../../proxy/links/link.service';
import { DashboardService } from '../../../proxy/dashboard/dashboard.service';
import { CollectionService } from '../../../proxy/collections/collection.service';
import { DashboardStatsDto } from '../../../proxy/dashboard/models';
import { CollectionDto } from '../../../proxy/collections/models';
import { ThemeService } from '../../services/theme.service';
import { LinkDto, LinkFilterDto } from 'src/app/proxy/links/dtos/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    NgxDatatableModule,
    NgbDropdownModule,
    NgbDatepickerModule,
    SidebarComponent,
    StatsCardComponent,
    LocalizationModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  links: LinkDto[] = [];
  stats: DashboardStatsDto | null = null;
  collections: CollectionDto[] = [];
  loading = false;
  searchTerm = '';
  selectedCollectionId: string | undefined = '';

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

  fromDate: NgbDateStruct | null = null;
  toDate: NgbDateStruct | null = null;

  constructor(
    private linkService: LinkService,
    private dashboardService: DashboardService,
    private collectionService: CollectionService,
    private modalService: NgbModal,
    private offcanvasService: NgbOffcanvas,
    private confirmation: ConfirmationService,
    private toaster: ToasterService,
    public localization: LocalizationService,
    public themeService: ThemeService,
  ) {}

  ngOnInit(): void {
    this.loadData();
    this.loadCollections();
  }

  loadData() {
    this.loading = true;

    // Load Stats
    this.dashboardService.getStats().subscribe({
      next: res => {
        this.stats = res;
      },
      error: err => {
        console.error('Failed to load stats', err);
      },
    });

    // Load Links
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
    this.loadData();
  }

  onSortChange() {
    this.filter.sorting = this.selectedSort;
    this.loadData();
  }

  loadCollections() {
    this.collectionService.getList().subscribe({
      next: res => {
        this.collections = res.items;
      },
      error: err => {
        console.error('Failed to load collections', err);
      },
    });
  }

  onCollectionFilterChange() {
    // Convert empty string to undefined so it's not sent in the request
    this.filter.collectionId =
      this.selectedCollectionId === '' ? undefined : this.selectedCollectionId;
    this.loadData();
  }

  filterByCollection(collectionId: string | undefined) {
    this.filter.collectionId = collectionId;
    this.loadData();
  }

  filterByFavorites() {
    this.filter.isFavorite = this.filter.isFavorite ? undefined : true;
    this.loadData();
  }

  // Modal Actions
  addLink() {
    const modalRef = this.modalService.open(LinkModalComponent, {
      size: 'lg',
      centered: true,
      backdrop: 'static',
    });
    modalRef.componentInstance.mode = 'create';

    modalRef.result.then(
      result => {
        if (result) {
          // Link created successfully, reload data
          this.loadData();
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
          // Link updated, reload data
          this.loadData();
        }
      },
      () => {
        /* Modal dismissed */
      },
    );
  }

  onDateSelect() {
    if (!this.fromDate && !this.toDate) {
      this.loadData();
      return;
    }

    // Client-side filtering for date range since API might not support it directly yet
    // Ideally this should be passed to the API
    this.loading = true;
    this.linkService.getList(this.filter).subscribe({
      next: res => {
        let filteredItems = res.items;

        if (this.fromDate) {
          const from = new Date(this.fromDate.year, this.fromDate.month - 1, this.fromDate.day);
          filteredItems = filteredItems.filter(item => new Date(item.creationTime!) >= from);
        }

        if (this.toDate) {
          const to = new Date(this.toDate.year, this.toDate.month - 1, this.toDate.day);
          // Set to end of day
          to.setHours(23, 59, 59, 999);
          filteredItems = filteredItems.filter(item => new Date(item.creationTime!) <= to);
        }

        this.links = filteredItems;
        this.loading = false;
      },
      error: err => {
        console.error('Failed to load links', err);
        this.loading = false;
      },
    });
  }

  deleteLink(link: LinkDto) {
    this.confirmation
      .warn(
        `Are you sure you want to delete "${link.title}"? This action cannot be undone.`,
        'Delete Link',
      )
      .subscribe(status => {
        if (status === 'confirm') {
          this.linkService.delete(link.id).subscribe({
            next: () => {
              this.links = this.links.filter(l => l.id !== link.id);
              this.loadData();
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
        // Refresh stats to update favorite count
        this.dashboardService.getStats().subscribe(stats => (this.stats = stats));
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
    const offcanvasRef = this.offcanvasService.open(SidebarComponent, {
      position: 'start',
      panelClass: 'sidebar-offcanvas',
    });
  }
}
