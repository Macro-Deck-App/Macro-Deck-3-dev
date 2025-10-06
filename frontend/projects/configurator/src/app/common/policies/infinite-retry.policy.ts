import {RetryContext} from '@microsoft/signalr';

export class InfiniteRetryPolicy {
  nextRetryDelayInMilliseconds(retryContext: RetryContext) {
    console.warn(`Reconnecting... attempt #${retryContext.previousRetryCount + 1}`);
    return 5000; // Try every 5 seconds forever
  }
}
