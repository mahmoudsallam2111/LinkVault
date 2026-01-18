import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LinkCardComponent } from '../dashboard/link-card/link-card.component';
import { LinkModalComponent } from '../dashboard/link-modal/link-modal.component';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { LinkService } from '../../../proxy/links/link.service';
import { LinkDto, LinkFilterDto } from '../../../proxy/links/models';

import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
    selector: 'app-favorites',
    standalone: true,
    imports: [CommonModule, FormsModule, LinkCardComponent, SidebarComponent],
    templateUrl: './favorites.component.html',
    styleUrls: ['./favorites.component.css']
})
export class FavoritesComponent implements OnInit {
    links: LinkDto[] = [];
    loading = false;
    searchTerm = '';

    filter: LinkFilterDto = {
        skipCount: 0,
        maxResultCount: 100,
        includeDeleted: false,
        isFavorite: true
    };

    constructor(
        private linkService: LinkService,
        private modalService: NgbModal
    ) { }

    ngOnInit(): void {
        this.loadData();
    }

    loadData() {
        this.loading = true;
        this.linkService.getList(this.filter).subscribe({
            next: (res) => {
                this.links = res.items;
                this.loading = false;
            },
            error: (err) => {
                console.error('Failed to load favorites', err);
                this.loading = false;
            }
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
        const modalRef = this.modalService.open(ConfirmDialogComponent, {
            centered: true
        });
        modalRef.componentInstance.title = 'Delete Link';
        modalRef.componentInstance.message = `Are you sure you want to delete "${link.title}"?`;
        modalRef.componentInstance.confirmText = 'Delete';
        modalRef.componentInstance.iconClass = 'fas fa-trash-alt text-danger';

        modalRef.result.then((confirmed) => {
            if (confirmed) {
                this.linkService.delete(link.id).subscribe(() => {
                    this.links = this.links.filter(l => l.id !== link.id);
                });
            }
        }, () => { });
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
                // If favorite status changed in edit, remove from list ?? 
                // Result contains updated link. Check isFavorite.
                if (!result.isFavorite) {
                    this.links = this.links.filter(l => l.id !== link.id);
                } else {
                    this.loadData();
                }
            }
        }, () => { });
    }

    visitLink(link: LinkDto) {
        this.linkService.incrementVisit(link.id).subscribe();
        window.open(link.url, '_blank');
    }
}
