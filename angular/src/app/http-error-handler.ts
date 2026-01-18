// import { HttpErrorResponse } from '@angular/common/http';
// import { Injector } from '@angular/core';
// import { HTTP_ERROR_HANDLER } from '@abp/ng.theme.shared';
// import { EMPTY, Observable } from 'rxjs';

// /**
//  * Custom HTTP error handler that suppresses error modals for network errors (status 0).
//  * Status 0 typically indicates a network error, CORS issue, or connection refused.
//  * These errors are often transient and shouldn't show an error modal to the user.
//  */
// export function customHttpErrorHandler(injector: Injector) {
//     return (error: HttpErrorResponse): Observable<any> | null => {
//         // Suppress error modal for network errors (status 0)
//         // These are typically connection refused, CORS issues, or timeout errors
//         if (error.status === 0) {
//             console.warn('Network error occurred (status 0), suppressing error modal:', error.url);
//             return EMPTY; // Return EMPTY to indicate the error was handled
//         }

//         // Return null to let ABP's default error handler deal with other errors
//         return null;
//     };
// }

// /**
//  * Provider for the custom HTTP error handler.
//  * Use this in the providers array of app.config.ts
//  */
// export const CUSTOM_HTTP_ERROR_HANDLER_PROVIDER = {
//     provide: HTTP_ERROR_HANDLER,
//     useFactory: customHttpErrorHandler,
//     deps: [Injector]
// };
