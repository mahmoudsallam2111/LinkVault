import { Component, OnInit } from '@angular/core';
import { DynamicLayoutComponent } from '@abp/ng.core';
import { LoaderBarComponent } from '@abp/ng.theme.shared';
import { SignalRService } from './link-vault/services/signalr.service';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <abp-dynamic-layout />
  `,
  imports: [LoaderBarComponent, DynamicLayoutComponent],
})
export class AppComponent implements OnInit {
  constructor(private signalRService: SignalRService) { }

  ngOnInit() {
    this.signalRService.startConnection();
  }
}
