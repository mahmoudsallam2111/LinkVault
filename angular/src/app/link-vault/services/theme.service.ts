import { Injectable, Renderer2, RendererFactory2 } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type Theme = 'light' | 'dark';

@Injectable({
    providedIn: 'root'
})
export class ThemeService {
    private renderer: Renderer2;
    private currentTheme = new BehaviorSubject<Theme>(this.getStoredTheme());

    theme$ = this.currentTheme.asObservable();

    constructor(rendererFactory: RendererFactory2) {
        this.renderer = rendererFactory.createRenderer(null, null);
        this.applyTheme(this.currentTheme.value);
    }

    private getStoredTheme(): Theme {
        const stored = localStorage.getItem('linkvault-theme') as Theme;
        if (stored) {
            return stored;
        }
        // Check system preference
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            return 'dark';
        }
        return 'light';
    }

    get isDarkMode(): boolean {
        return this.currentTheme.value === 'dark';
    }

    toggleTheme(): void {
        const newTheme: Theme = this.currentTheme.value === 'light' ? 'dark' : 'light';
        this.setTheme(newTheme);
    }

    setTheme(theme: Theme): void {
        localStorage.setItem('linkvault-theme', theme);
        this.currentTheme.next(theme);
        this.applyTheme(theme);
    }

    private applyTheme(theme: Theme): void {
        const body = document.body;
        if (theme === 'dark') {
            this.renderer.addClass(body, 'dark-theme');
            this.renderer.removeClass(body, 'light-theme');
        } else {
            this.renderer.addClass(body, 'light-theme');
            this.renderer.removeClass(body, 'dark-theme');
        }
    }
}
