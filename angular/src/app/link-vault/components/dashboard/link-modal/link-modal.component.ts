import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { LocalizationModule } from '@abp/ng.core';
import { LinkService } from '../../../../proxy/links/link.service';
import { CollectionService } from '../../../../proxy/collections/collection.service';
import { CollectionDto } from '../../../../proxy/collections/models';
import { CreateUpdateLinkDto, LinkDto } from 'src/app/proxy/links/dtos';

@Component({
  selector: 'app-link-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, LocalizationModule],
  templateUrl: './link-modal.component.html',
  styleUrls: ['./link-modal.component.css'],
})
export class LinkModalComponent implements OnInit {
  @Input() link?: LinkDto;
  @Input() mode: 'create' | 'edit' = 'create';

  form: CreateUpdateLinkDto = {
    url: '',
    title: '',
    description: '',
    favicon: '',
    isFavorite: false,
    collectionId: undefined,
    tagNames: [],
  };

  tagsInput = '';
  collections: CollectionDto[] = [];
  loading = false;
  fetchingMetadata = false;
  error = '';

  constructor(
    public activeModal: NgbActiveModal,
    private linkService: LinkService,
    private collectionService: CollectionService,
  ) {}

  ngOnInit(): void {
    this.loadCollections();

    if (this.mode === 'edit' && this.link) {
      this.form = {
        url: this.link.url || '',
        title: this.link.title || '',
        description: this.link.description || '',
        favicon: this.link.favicon || '',
        isFavorite: this.link.isFavorite || false,
        collectionId: this.link.collectionId || undefined,
        tagNames: this.link.tags?.map(t => t.name || '') || [],
      };
      this.tagsInput = this.form.tagNames?.join(', ') || '';
    }
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

  fetchMetadata() {
    if (!this.form.url) {
      this.error = 'Please enter a URL first';
      return;
    }

    this.fetchingMetadata = true;
    this.error = '';

    this.linkService.fetchMetadata(this.form.url).subscribe({
      next: metadata => {
        this.form.title = metadata.title || this.form.title;
        this.form.description = metadata.description || this.form.description;
        this.form.favicon = metadata.favicon || this.form.favicon;
        this.fetchingMetadata = false;
      },
      error: err => {
        this.error = 'Failed to fetch metadata. You can still enter details manually.';
        this.fetchingMetadata = false;
        console.error('Metadata fetch error', err);
      },
    });
  }

  save() {
    if (!this.form.url || !this.form.title) {
      this.error = 'URL and Title are required';
      return;
    }

    this.loading = true;
    this.error = '';

    // Parse tags from comma-separated input
    this.form.tagNames = this.tagsInput
      .split(',')
      .map(t => t.trim())
      .filter(t => t.length > 0);

    if (this.mode === 'create') {
      this.linkService.create(this.form).subscribe({
        next: result => {
          this.loading = false;
          this.activeModal.close(result);
        },
        error: err => {
          this.loading = false;
          this.error = 'Failed to create link. Please try again.';
          console.error('Create link error', err);
        },
      });
    } else if (this.link?.id) {
      this.linkService.update(this.link.id, this.form).subscribe({
        next: result => {
          this.loading = false;
          this.activeModal.close(result);
        },
        error: err => {
          this.loading = false;
          this.error = 'Failed to update link. Please try again.';
          console.error('Update link error', err);
        },
      });
    }
  }

  cancel() {
    this.activeModal.dismiss('cancel');
  }
}
