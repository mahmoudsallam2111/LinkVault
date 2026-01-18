import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-stats-card',
    standalone: true,
    imports: [CommonModule],
    template: `
    <div class="card border-0 text-white h-100 shadow-sm transition-hover" 
         [ngClass]="bgClass" [style.background]="bgStyle">
      <div class="card-body p-4 d-flex flex-column justify-content-between">
        <div>
          <h2 class="fw-bold display-5 mb-0">{{ value }}</h2>
          <p class="mb-0 opacity-75 small text-uppercase fw-semibold">{{ title }}</p>
        </div>
        <!-- Optional icon or decor could go here -->
      </div>
    </div>
  `,
    styles: [`
    .transition-hover {
      transition: transform 0.2s ease, box-shadow 0.2s ease;
    }
    .transition-hover:hover {
      transform: translateY(-5px);
      box-shadow: 0 10px 20px rgba(0,0,0,0.1) !important;
    }
  `]
})
export class StatsCardComponent {
    @Input() title: string = '';
    @Input() value: number | string = 0;
    @Input() bgClass: string = 'bg-primary';
    @Input() bgStyle: string = '';
}
