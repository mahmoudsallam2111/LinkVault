import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CollectionService } from '../../../../proxy/collections/collection.service';
import { CollectionDto, CreateUpdateCollectionDto } from '../../../../proxy/collections/models';

@Component({
    selector: 'app-collection-modal',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './collection-modal.component.html',
    styleUrls: ['./collection-modal.component.css']
})
export class CollectionModalComponent implements OnInit {
    @Input() collection?: CollectionDto;
    @Input() mode: 'create' | 'edit' = 'create';

    form: CreateUpdateCollectionDto = {
        name: '',
        color: '#696cff',
        icon: 'folder',
        parentId: undefined,
        order: 0
    };

    collections: CollectionDto[] = [];
    loading = false;
    error = '';

    // Predefined colors
    colorOptions = [
        '#696cff', '#ff6384', '#03c3ec', '#2ecc71', '#ffab00',
        '#ff3e1d', '#8b5cf6', '#ec4899', '#06b6d4', '#84cc16'
    ];

    // Predefined icons
    iconOptions = [
        'folder', 'briefcase', 'book', 'star', 'heart',
        'bookmark', 'archive', 'box', 'code', 'globe'
    ];

    constructor(
        public activeModal: NgbActiveModal,
        private collectionService: CollectionService
    ) { }

    ngOnInit(): void {
        this.loadCollections();

        if (this.mode === 'edit' && this.collection) {
            this.form = {
                name: this.collection.name || '',
                color: this.collection.color || '#696cff',
                icon: this.collection.icon || 'folder',
                parentId: this.collection.parentId || undefined,
                order: this.collection.order || 0
            };
        }
    }

    loadCollections() {
        this.collectionService.getList().subscribe({
            next: (res) => {
                // Filter out current collection if editing (can't be its own parent)
                this.collections = res.items.filter(c => c.id !== this.collection?.id);
            },
            error: (err) => {
                console.error('Failed to load collections', err);
            }
        });
    }

    selectColor(color: string) {
        this.form.color = color;
    }

    selectIcon(icon: string) {
        this.form.icon = icon;
    }

    save() {
        if (!this.form.name?.trim()) {
            this.error = 'Collection name is required';
            return;
        }

        this.loading = true;
        this.error = '';

        if (this.mode === 'create') {
            this.collectionService.create(this.form).subscribe({
                next: (result) => {
                    this.loading = false;
                    this.activeModal.close(result);
                },
                error: (err) => {
                    this.loading = false;
                    this.error = 'Failed to create collection. Please try again.';
                    console.error('Create collection error', err);
                }
            });
        } else if (this.collection?.id) {
            this.collectionService.update(this.collection.id, this.form).subscribe({
                next: (result) => {
                    this.loading = false;
                    this.activeModal.close(result);
                },
                error: (err) => {
                    this.loading = false;
                    this.error = 'Failed to update collection. Please try again.';
                    console.error('Update collection error', err);
                }
            });
        }
    }

    cancel() {
        this.activeModal.dismiss('cancel');
    }
}
