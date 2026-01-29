import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-stats-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card border-0 text-white h-100 shadow-lg transition-hover overflow-hidden position-relative rounded-4" 
         [ngClass]="bgClass" [style.background]="bgStyle">
      
      <!-- Subtle internal border for definition -->
      <div class="position-absolute top-0 start-0 w-100 h-100 rounded-4 border border-white border-opacity-25 pointer-events-none"></div>

      <!-- Decorative huge icon in background - More subtle now -->
      <div class="position-absolute" style="right: -20px; bottom: -40px; opacity: 0.1; transform: rotate(-10deg); pointer-events: none;">
        <i [class]="icon" style="font-size: 10rem;"></i>
      </div>

      <div class="card-body p-4 position-relative z-1 d-flex flex-column justify-content-between">
        <div class="d-flex justify-content-between align-items-start mb-3">
            <div>
                 <p class="mb-2 opacity-75 text-uppercase fw-bold ls-2" style="font-size: 0.7rem;">{{ title }}</p>
                 <h2 class="fw-bolder display-4 mb-0 tracking-tight text-shadow-sm">{{ value }}</h2>
            </div>
            
            <!-- Glassy Icon Container -->
            <div class="icon-circle shadow-sm backdrop-blur d-flex align-items-center justify-content-center rounded-3" 
                 style="width: 54px; height: 54px; background: rgba(255, 255, 255, 0.2); border: 1px solid rgba(255, 255, 255, 0.3);">
                <i [class]="icon" class="fs-3 text-white"></i>
            </div>
        </div>
        
        <div class="mt-2 pt-3 border-top border-white border-opacity-10">
             <small class="opacity-90 d-flex align-items-center fw-medium" style="font-size: 0.85rem;">
                <span class="badge bg-white bg-opacity-20 rounded-pill me-2 px-2 py-1">
                    <i class="fas fa-arrow-up text-white" style="font-size: 0.7em;"></i>
                </span>
                <span>Active</span>
             </small>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .transition-hover {
      transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    }
    .transition-hover:hover {
      transform: translateY(-8px);
      box-shadow: 0 20px 40px rgba(0,0,0,0.2) !important;
    }
    .ls-2 {
        letter-spacing: 2px;
    }
    .tracking-tight {
        letter-spacing: -0.05em;
    }
    .text-shadow-sm {
        text-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    .backdrop-blur {
        backdrop-filter: blur(8px);
        -webkit-backdrop-filter: blur(8px);
    }
    .pointer-events-none {
        pointer-events: none;
    }
  `]
})
export class StatsCardComponent {
  @Input() title: string = '';
  @Input() value: number | string = 0;
  @Input() bgClass: string = 'bg-primary';
  @Input() bgStyle: string = '';
  @Input() icon: string = '';
}
