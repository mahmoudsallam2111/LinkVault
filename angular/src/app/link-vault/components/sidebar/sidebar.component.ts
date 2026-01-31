import { Component, OnInit, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LocalizationModule, PermissionService } from '@abp/ng.core';
import { NgbModal, NgbActiveOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { CollectionService } from '../../../proxy/collections/collection.service';
import { TagService } from '../../../proxy/tags/tag.service';
import { DashboardService } from '../../../proxy/dashboard/dashboard.service';
import { CollectionDto } from '../../../proxy/collections/models';
import { TagDto } from '../../../proxy/tags/models';
import { CollectionModalComponent } from './collection-modal/collection-modal.component';
import { ConfirmationService, ToasterService } from '@abp/ng.theme.shared';
import { ThemeService } from '../../services/theme.service';

@Component({
    selector: 'app-sidebar',
    standalone: true,
    imports: [CommonModule, RouterModule, LocalizationModule],
    templateUrl: './sidebar.component.html',
    styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
    collections: CollectionDto[] = [];
    tags: TagDto[] = [];

    quickAccess = [
        { labelKey: 'LinkVault::AllLinks', icon: 'fas fa-thumbtack', route: '/', count: 0, activeClass: 'text-primary bg-primary-subtle' },
        { labelKey: 'LinkVault::Favorites', icon: 'fas fa-star', route: '/favorites', count: 0, activeClass: 'text-warning bg-warning-subtle' },
        { labelKey: 'LinkVault::Recent', icon: 'fas fa-clock', route: '/recent', count: 0, activeClass: 'text-info bg-info-subtle' },
        { labelKey: 'LinkVault::Reminders', icon: 'fas fa-bell', route: '/reminders', count: 0, activeClass: 'text-info bg-info-subtle' },
        { labelKey: 'LinkVault::Trash', icon: 'fas fa-trash', route: '/trash', count: 0, activeClass: 'text-danger bg-danger-subtle' }
    ];

    adminLinks = [
        { label: 'Users', icon: 'fas fa-users', route: '/identity/users', permission: 'AbpIdentity.Users' },
        { label: 'Roles', icon: 'fas fa-user-shield', route: '/identity/roles', permission: 'AbpIdentity.Roles' }
    ];

    canManageUsers = false;
    canManageRoles = false;

    constructor(
        private collectionService: CollectionService,
        private tagService: TagService,
        private dashboardService: DashboardService,
        private permissionService: PermissionService,
        private modalService: NgbModal,
        private confirmation: ConfirmationService,
        private toaster: ToasterService,
        public themeService: ThemeService,
        @Optional() private activeOffcanvas: NgbActiveOffcanvas
    ) { }

    closeSidebar() {
        if (this.activeOffcanvas) {
            this.activeOffcanvas.dismiss('Navigation');
        }
    }

    ngOnInit(): void {
        this.loadCollections();
        this.loadTags();
        this.loadCounts();
        this.checkAdminPermissions();

        if (!this.canManageUsers && !this.canManageRoles) {
            this.quickAccess = this.quickAccess.filter(item => item.labelKey !== 'LinkVault::Reminders');
        }
    }

    rootCollections: CollectionDto[] = [];

    // ...

    loadCollections() {
        this.collectionService.getTree().subscribe({
            next: (res) => {
                this.collections = res.items;
            },
            error: (err) => {
                console.error('Failed to load collections', err);
            }
        });
    }

    loadTags() {
        this.tagService.getList().subscribe({
            next: (res) => {
                this.tags = res.items;
            },
            error: (err) => {
                console.error('Failed to load tags', err);
            }
        });
    }

    loadCounts() {
        this.dashboardService.getStats().subscribe({
            next: (stats) => {
                this.quickAccess[0].count = stats.totalLinks;
                this.quickAccess[1].count = stats.favoriteCount;
                this.quickAccess[2].count = stats.linksAddedThisWeek;
            },
            error: (err) => {
                console.error('Failed to load counts', err);
            }
        });
    }

    checkAdminPermissions() {
        this.canManageUsers = this.permissionService.getGrantedPolicy('AbpIdentity.Users');
        this.canManageRoles = this.permissionService.getGrantedPolicy('AbpIdentity.Roles');
    }

    get showAdminSection(): boolean {
        return this.canManageUsers || this.canManageRoles;
    }

    get isDarkMode(): boolean {
        return this.themeService.isDarkMode;
    }

    toggleTheme(): void {
        this.themeService.toggleTheme();
    }

    // Collection Modal Actions
    addCollection() {
        const modalRef = this.modalService.open(CollectionModalComponent, {
            centered: true,
            backdrop: 'static'
        });
        modalRef.componentInstance.mode = 'create';

        modalRef.result.then(
            (result) => {
                if (result) {
                    this.loadCollections();
                }
            },
            () => { /* Modal dismissed */ }
        );
    }

    editCollection(collection: CollectionDto, event: Event) {
        event.stopPropagation();
        const modalRef = this.modalService.open(CollectionModalComponent, {
            centered: true,
            backdrop: 'static'
        });
        modalRef.componentInstance.mode = 'edit';
        modalRef.componentInstance.collection = collection;

        modalRef.result.then(
            (result) => {
                if (result) {
                    this.loadCollections();
                }
            },
            () => { /* Modal dismissed */ }
        );
    }

    deleteCollection(collection: CollectionDto, event: Event) {
        event.stopPropagation();
        this.confirmation
            .warn(`Are you sure you want to delete "${collection.name}"? Links in this collection will not be deleted.`, 'Delete Collection')
            .subscribe((status) => {
                if (status === 'confirm') {
                    this.collectionService.delete(collection.id).subscribe({
                        next: () => {
                            this.loadCollections();
                            this.toaster.success('Collection deleted successfully');
                        },
                        error: (err) => {
                            console.error('Failed to delete collection', err);
                            this.toaster.error('Failed to delete collection');
                        }
                    });
                }
            });
    }
}
