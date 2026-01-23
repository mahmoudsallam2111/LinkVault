import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { CollectionService } from '../../../proxy/collections/collection.service';
import { PublicCollectionDto, PublicLinkDto } from '../../../proxy/collections/models';

@Component({
  selector: 'app-public-collection',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './public-collection.component.html',
  styleUrls: ['./public-collection.component.css'],
})
export class PublicCollectionComponent implements OnInit {
  collection: PublicCollectionDto | null = null;
  loading = true;
  error = false;
  token: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private collectionService: CollectionService,
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.token = params['token'];
      if (this.token) {
        this.loadCollection();
      } else {
        this.error = true;
        this.loading = false;
      }
    });
  }

  loadCollection() {
    this.loading = true;
    this.collectionService.getByShareToken(this.token).subscribe({
      next: collection => {
        if (collection) {
          this.collection = collection;
        } else {
          this.error = true;
        }
        this.loading = false;
      },
      error: err => {
        console.error('Failed to load public collection', err);
        this.error = true;
        this.loading = false;
      },
    });
  }

  visitLink(link: PublicLinkDto) {
    if (link.url) {
      window.open(link.url, '_blank');
    }
  }

  goHome() {
    this.router.navigate(['/']);
  }

  getDomain(url: string | undefined): string {
    if (!url) return '';
    try {
      return new URL(url).hostname;
    } catch {
      return url;
    }
  }
}
