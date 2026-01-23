import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal, NgbDropdownModule, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { LocalizationService } from '@abp/ng.core';
import { LinkCardComponent } from '../dashboard/link-card/link-card.component';
import { LinkModalComponent } from '../dashboard/link-modal/link-modal.component';
import { ConfirmationService, ToasterService } from '@abp/ng.theme.shared';
import { LinkService } from '../../../proxy/links/link.service';
import { LinkDto } from '../../../proxy/links/models';

import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
    selector: 'app-recent',
    standalone: true,
    imports: [CommonModule, FormsModule, NgxDatatableModule, NgbDropdownModule, SidebarComponent],
    templateUrl: './recent.component.html',
    styleUrls: ['./recent.component.css']
})
export class RecentComponent implements OnInit {
    links: LinkDto[] = [];
    loading = false;
    searchTerm = '';

    constructor(
        private linkService: LinkService,
        private modalService: NgbModal,
        private offcanvasService: NgbOffcanvas,
        private confirmation: ConfirmationService,
        private toaster: ToasterService,
        public localization: LocalizationService
    ) { }

    ngOnInit(): void {
        this.loadData();
    }

    loadData() {
        this.loading = true;
        this.linkService.getAddedToday().subscribe({
            next: (res) => {
                this.links = res.items;
                this.loading = false;
            },
            error: (err) => {
                console.error('Failed to load recent links', err);
                this.loading = false;
            }
        });
    }

    onSearch(term: string) {
        // Client-side filtering for simplicity since getAddedToday doesn't support filter param yet (ListResultDto)
        if (!term) {
            this.loadData();
            return;
        }
        this.links = this.links.filter(l =>
            l.title?.toLowerCase().includes(term.toLowerCase()) ||
            l.url?.toLowerCase().includes(term.toLowerCase())
        );
    }

    toggleFavorite(link: LinkDto) {
        this.linkService.toggleFavorite(link.id).subscribe((res) => {
            link.isFavorite = res.isFavorite;
        });
    }

    deleteLink(link: LinkDto) {
        this.confirmation
            .warn(`Are you sure you want to delete "${link.title}"?`, 'Delete Link')
            .subscribe((status) => {
                if (status === 'confirm') {
                    this.linkService.delete(link.id).subscribe({
                        next: () => {
                            this.links = this.links.filter(l => l.id !== link.id);
                            this.toaster.success('Link deleted successfully');
                        },
                        error: (err) => {
                            console.error('Failed to delete link', err);
                            this.toaster.error('Failed to delete link');
                        }
                    });
                }
            });
    }

    editLink(link: LinkDto) {
        const modalRef = this.modalService.open(LinkModalComponent, {
            size: 'lg',
            centered: true,
            backdrop: 'static'
        });
        modalRef.componentInstance.mode = 'edit';
        modalRef.componentInstance.link = link;

        modalRef.result.then((result) => {
            if (result) {
                this.loadData(); // Reload to reflect changes
            }
        }, () => { });
    }

    visitLink(link: LinkDto) {
        this.linkService.incrementVisit(link.id).subscribe();
        window.open(link.url, '_blank');
    }

    openMobileSidebar() {
        this.offcanvasService.open(SidebarComponent, {
            position: 'start',
            panelClass: 'sidebar-offcanvas'
        });
    }
}
