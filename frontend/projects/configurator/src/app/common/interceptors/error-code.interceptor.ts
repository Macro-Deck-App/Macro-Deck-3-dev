import { ErrorCodeExceptionModel } from "../models/error-code-exception.model";
import {catchError, Observable, throwError} from 'rxjs';
import {HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Injectable} from '@angular/core';

@Injectable()
export class ErrorCodeInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {

        if (error.error && typeof error.error === 'object' && 'code' in error.error && 'message' in error.error) {
          const { code, message } = error.error;
          return throwError(() => new ErrorCodeExceptionModel(code, message));
        }

        // fallback – rethrow original error
        return throwError(() => error);
      })
    );
  }
}
