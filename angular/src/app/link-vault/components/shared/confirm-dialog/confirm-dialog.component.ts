import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-confirm-dialog',
    standalone: true,
    imports: [CommonModule],
    template: `
        <div class="modal-header border-0">
            <h5 class="modal-title fw-bold">
                <i [class]="iconClass + ' me-2'"></i>{{ title }}
            </h5>
            <button type="button" class="btn-close" (click)="cancel()"></button>
        </div>
        <div class="modal-body">
            <p class="mb-0 text-muted">{{ message }}</p>
        </div>
        <div class="modal-footer border-0">
            <button type="button" class="btn btn-light px-4" (click)="cancel()">
                {{ cancelText }}
            </button>
            <button type="button" [class]="'btn px-4 ' + confirmBtnClass" (click)="confirm()">
                {{ confirmText }}
            </button>
        </div>
    `,
    styles: [`
        .btn-danger {
            background: linear-gradient(135deg, #ff3e1d 0%, #dc2626 100%);
            border: none;
        }
        .btn-primary {
            background: linear-gradient(135deg, #696cff 0%, #4338ca 100%);
            border: none;
        }
    `]
})
export class ConfirmDialogComponent {
    @Input() title = 'Confirm';
    @Input() message = 'Are you sure?';
    @Input() confirmText = 'Confirm';
    @Input() cancelText = 'Cancel';
    @Input() confirmBtnClass = 'btn-danger';
    @Input() iconClass = 'fas fa-question-circle text-warning';

    constructor(public activeModal: NgbActiveModal) { }

    confirm() {
        this.activeModal.close(true);
    }

    cancel() {
        this.activeModal.dismiss(false);
    }
}
