import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { StatsCardComponent } from './stats-card/stats-card.component';
import { LinkCardComponent } from './link-card/link-card.component';
import { LinkModalComponent } from './link-modal/link-modal.component';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { LinkService } from '../../../proxy/links/link.service';
import { DashboardService } from '../../../proxy/dashboard/dashboard.service';
import { LinkDto, LinkFilterDto } from '../../../proxy/links/models';
import { DashboardStatsDto } from '../../../proxy/dashboard/models';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [CommonModule, FormsModule, SidebarComponent, StatsCardComponent, LinkCardComponent],
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
    links: LinkDto[] = [];
    stats: DashboardStatsDto | null = null;
    loading = false;
    searchTerm = '';

    filter: LinkFilterDto = {
        skipCount: 0,
        maxResultCount: 100,
        includeDeleted: false
    };

    // Sort options
    sortOptions = [
        { value: 'creationTime desc', label: 'Newest First' },
        { value: 'creationTime asc', label: 'Oldest First' },
        { value: 'title asc', label: 'Title A-Z' },
        { value: 'title desc', label: 'Title Z-A' },
        { value: 'visitCount desc', label: 'Most Visited' }
    ];
    selectedSort = 'creationTime desc';

    constructor(
        private linkService: LinkService,
        private dashboardService: DashboardService,
        private modalService: NgbModal
    ) { }

    ngOnInit(): void {
        this.loadData();
    }

    loadData() {
        this.loading = true;

        // Load Stats
        this.dashboardService.getStats().subscribe({
            next: (res) => {
                this.stats = res;
            },
            error: (err) => {
                console.error('Failed to load stats', err);
            }
        });

        // Load Links
        this.filter.sorting = this.selectedSort;
        this.linkService.getList(this.filter).subscribe({
            next: (res) => {
                this.links = res.items;
                this.loading = false;
            },
            error: (err) => {
                console.error('Failed to load links', err);
                this.loading = false;
            }
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
            backdrop: 'static'
        });
        modalRef.componentInstance.mode = 'create';

        modalRef.result.then(
            (result) => {
                if (result) {
                    // Link created successfully, reload data
                    this.loadData();
                }
            },
            () => { /* Modal dismissed */ }
        );
    }

    editLink(link: LinkDto) {
        const modalRef = this.modalService.open(LinkModalComponent, {
            size: 'lg',
            centered: true,
            backdrop: 'static'
        });
        modalRef.componentInstance.mode = 'edit';
        modalRef.componentInstance.link = link;

        modalRef.result.then(
            (result) => {
                if (result) {
                    // Link updated, reload data
                    this.loadData();
                }
            },
            () => { /* Modal dismissed */ }
        );
    }

    deleteLink(link: LinkDto) {
        const modalRef = this.modalService.open(ConfirmDialogComponent, {
            centered: true
        });
        modalRef.componentInstance.title = 'Delete Link';
        modalRef.componentInstance.message = `Are you sure you want to delete "${link.title}"? This action cannot be undone.`;
        modalRef.componentInstance.confirmText = 'Delete';
        modalRef.componentInstance.iconClass = 'fas fa-trash-alt text-danger';

        modalRef.result.then(
            (confirmed) => {
                if (confirmed) {
                    this.linkService.delete(link.id).subscribe({
                        next: () => {
                            this.links = this.links.filter(l => l.id !== link.id);
                            this.loadData(); // Refresh stats
                        },
                        error: (err) => {
                            console.error('Failed to delete link', err);
                        }
                    });
                }
            },
            () => { /* Modal dismissed */ }
        );
    }

    toggleFavorite(link: LinkDto) {
        this.linkService.toggleFavorite(link.id).subscribe({
            next: (updatedLink) => {
                link.isFavorite = updatedLink.isFavorite;
                // Refresh stats to update favorite count
                this.dashboardService.getStats().subscribe(stats => this.stats = stats);
            },
            error: (err) => {
                console.error('Failed to toggle favorite', err);
            }
        });
    }

    visitLink(link: LinkDto) {
        this.linkService.incrementVisit(link.id).subscribe({
            next: (updatedLink) => {
                link.visitCount = updatedLink.visitCount;
            }
        });
        window.open(link.url, '_blank');
    }
}

