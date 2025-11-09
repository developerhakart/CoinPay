import { env } from '@/config/env';

// Type for the SDK (will be loaded dynamically)
type W3SSdkType = any;

class CircleService {
  private sdk: W3SSdkType = null;
  private initialized: boolean = false;

  async initialize(): Promise<void> {
    if (this.initialized) return;

    if (!env.circleAppId) {
      console.warn('Circle App ID not configured. Using mock mode.');
      return;
    }

    try {
      // Dynamic import - only load Circle SDK when actually needed
      const { W3SSdk } = await import('@circle-fin/w3s-pw-web-sdk');

      this.sdk = new W3SSdk({
        appSettings: {
          appId: env.circleAppId,
        },
      });

      this.initialized = true;
      console.log('Circle SDK initialized');
    } catch (error) {
      console.error('Circle SDK init failed:', error);
      throw error;
    }
  }

  async executeChallenge(challengeId: string, userToken: string, encryptionKey: string): Promise<any> {
    if (!this.initialized || !this.sdk) {
      throw new Error('Circle SDK not initialized');
    }

    // Set user authentication
    this.sdk.setAuthentication({
      userToken,
      encryptionKey,
    });

    // Execute challenge (opens passkey prompt)
    return new Promise((resolve, reject) => {
      this.sdk.execute(challengeId, (error: any, result: any) => {
        if (error) {
          reject(error);
        } else {
          resolve(result);
        }
      });
    });
  }

  isInitialized(): boolean {
    return this.initialized;
  }

  shouldUseMock(): boolean {
    return !env.useRealCircleSDK || !env.circleAppId;
  }
}

export const circleService = new CircleService();
export default circleService;
