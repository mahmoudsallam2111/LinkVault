import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Settings } from './settings';
import { ProfileSettings } from './profile-settings/profile-settings';
import { EmailPreferences } from './email-preferences/email-preferences';
import { Password } from './password/password';
import { AccountDeletion } from './account-deletion/account-deletion';

const routes: Routes = [
  {
    path: '',
    component: Settings,
    children: [
      { path: '', redirectTo: 'profile', pathMatch: 'full' },
      { path: 'profile', component: ProfileSettings },
      { path: 'email', component: EmailPreferences },
      { path: 'password', component: Password },
      { path: 'deletion', component: AccountDeletion }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SettingsRoutingModule { }
