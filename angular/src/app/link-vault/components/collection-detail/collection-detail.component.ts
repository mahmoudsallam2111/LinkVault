import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { NgbModal, NgbDropdownModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { LocalizationService } from '@abp/ng.core';
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
    imports: [CommonModule, FormsModule, RouterModule, NgxDatatableModule, NgbDropdownModule, SidebarComponent],
    templateUrl: './collection-detail.component.html',
    styleUrls: ['./collection-detail.component.css'],
})
export class CollectionDetailComponent implements OnInit {
    collection: CollectionDto | null = null;
    links: LinkDto[] = [];
    loading = false;
    searchTerm = '';
    collectionId: string = '';

    filter: LinkFilterDto = {
        skipCount: 0,
        maxResultCount: 100,
        includeDeleted: false,
    };

    // Sort options
    sortOptions = [
        { value: 'creationTime desc', label: 'Newest First' },
        { value: 'creationTime asc', label: 'Oldest First' },
        { value: 'title asc', label: 'Title A-Z' },
        { value: 'title desc', label: 'Title Z-A' },
        { value: 'visitCount desc', label: 'Most Visited' },
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
            panelClass: 'sidebar-offcanvas'
        });
    }
}
