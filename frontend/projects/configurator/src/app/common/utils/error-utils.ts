import {ErrorCodeExceptionModel} from '../models/error-code-exception.model';

export function isErrorCodeException(err: unknown): err is ErrorCodeExceptionModel {
  return err instanceof ErrorCodeExceptionModel;
}

export function applyToButton(err: unknown, component: { buttonState: string; errorMessage: string | null }) {
  if (isErrorCodeException(err)) {
    component.buttonState = 'error';
    component.errorMessage = err.message;
  } else {
    component.buttonState = 'error';
    component.errorMessage = 'Network or server error';
  }
}
