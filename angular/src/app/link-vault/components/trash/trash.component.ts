import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LinkCardComponent } from '../dashboard/link-card/link-card.component';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { LinkService } from '../../../proxy/links/link.service';
import { LinkDto } from '../../../proxy/links/models';

import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
    selector: 'app-trash',
    standalone: true,
    imports: [CommonModule, FormsModule, LinkCardComponent, SidebarComponent],
    templateUrl: './trash.component.html',
    styleUrls: ['./trash.component.css']
})
export class TrashComponent implements OnInit {
    links: LinkDto[] = [];
    loading = false;
    searchTerm = '';

    constructor(
        private linkService: LinkService,
        private modalService: NgbModal
    ) { }

    ngOnInit(): void {
        this.loadData();
    }

    loadData() {
        this.loading = true;
        // getTrash takes PagedAndSortedResultRequestDto
        this.linkService.getTrash({
            skipCount: 0,
            maxResultCount: 100,
            sorting: 'deletionTime desc'
        }).subscribe({
            next: (res) => {
                this.links = res.items;
                this.loading = false;
            },
            error: (err) => {
                console.error('Failed to load trash', err);
                this.loading = false;
            }
        });
    }

    onSearch(term: string) {
        if (!term) {
            this.loadData();
            return;
        }
        this.links = this.links.filter(l =>
            l.title?.toLowerCase().includes(term.toLowerCase()) ||
            l.url?.toLowerCase().includes(term.toLowerCase())
        );
    }

    restoreLink(link: LinkDto) {
        this.linkService.restore(link.id).subscribe({
            next: () => {
                this.links = this.links.filter(l => l.id !== link.id);
            },
            error: (err) => {
                console.error('Failed to restore link', err);
            }
        });
    }

    hardDeleteLink(link: LinkDto) {
        const modalRef = this.modalService.open(ConfirmDialogComponent, {
            centered: true
        });
        modalRef.componentInstance.title = 'Delete Forever';
        modalRef.componentInstance.message = `Are you sure you want to permanently delete "${link.title}"? This cannot be undone.`;
        modalRef.componentInstance.confirmText = 'Delete Forever';
        modalRef.componentInstance.iconClass = 'fas fa-exclamation-triangle text-danger';

        modalRef.result.then((confirmed) => {
            if (confirmed) {
                this.linkService.hardDelete(link.id).subscribe({
                    next: () => {
                        this.links = this.links.filter(l => l.id !== link.id);
                    },
                    error: (err) => {
                        console.error('Failed to delete link permanently', err);
                    }
                });
            }
        }, () => { });
    }
}
