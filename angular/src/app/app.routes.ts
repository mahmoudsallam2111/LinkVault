import { authGuard, permissionGuard, eLayoutType } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./link-vault/components/dashboard/dashboard.component').then(
        c => c.DashboardComponent,
      ),
    canActivate: [authGuard],
  },
  {
    path: 'favorites',
    loadComponent: () =>
      import('./link-vault/components/favorites/favorites.component').then(
        c => c.FavoritesComponent,
      ),
    canActivate: [authGuard],
  },
  {
    path: 'recent',
    loadComponent: () =>
      import('./link-vault/components/recent/recent.component').then(c => c.RecentComponent),
    canActivate: [authGuard],
  },
  {
    path: 'trash',
    loadComponent: () =>
      import('./link-vault/components/trash/trash.component').then(c => c.TrashComponent),
    canActivate: [authGuard],
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'tenant-management',
    loadChildren: () => import('@abp/ng.tenant-management').then(c => c.createRoutes()),
  },
  {
    path: 'settings',
    loadChildren: () => import('./settings/settings.module').then(m => m.SettingsModule),
    canActivate: [authGuard],
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
];
