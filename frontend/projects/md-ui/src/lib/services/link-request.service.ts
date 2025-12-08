import { Injectable } from '@angular/core';
import { LinkRequestMessage } from '../models';

/**
 * Provider function type for showing link requests
 */
export type LinkRequestProvider = (request: LinkRequestMessage) => Promise<boolean>;

/**
 * Service for handling link requests.
 * Uses a provider pattern to allow the consuming application to provide its own UI implementation.
 */
@Injectable({
  providedIn: 'root'
})
export class LinkRequestService {
  private provider: LinkRequestProvider | null = null;

  /**
   * Set the provider function that will handle link requests
   */
  setProvider(provider: LinkRequestProvider): void {
    this.provider = provider;
  }

  /**
   * Shows a link request and returns the user's response
   */
  async showLinkRequest(request: LinkRequestMessage): Promise<boolean> {
    if (!this.provider) {
      console.error('[LinkRequestService] No provider set!');
      return false;
    }

    return await this.provider(request);
  }
}
