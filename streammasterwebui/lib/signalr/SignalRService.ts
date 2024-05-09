import { isCSharpException } from '@lib/apiDefs';
import { addError } from '@lib/redux/hooks/messages';

import store from '@lib/redux/store';
import { baseHostURL, isDev } from '@lib/settings';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack';

class SignalRService extends EventTarget {
  private static instance: SignalRService;
  public hubConnection: HubConnection;
  private blacklistedMethods: string[] = ['GetLog', 'GetIconFromSource'];
  private whitelistedMethods: string[] = [];
  private listenerCounts: Map<string, number> = new Map<string, number>();

  private constructor() {
    super();

    const url = `${baseHostURL}/streammasterhub`;
    this.hubConnection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Error)
      .withUrl(url)
      .withHubProtocol(new MessagePackHubProtocol())
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          if (retryContext.elapsedMilliseconds < 60000) {
            return 2000;
          }
          return 2000;
        }
      })
      .build();

    this.hubConnection.onclose((callback) => {
      this.dispatchEvent(new CustomEvent('signalr_disconnected'));
      console.log('SignalR connection closed', callback);
      this.start();
    });

    this.start();
  }

  public get isConnected(): boolean {
    return this.hubConnection.state === HubConnectionState.Connected;
  }

  public get isDisConnected(): boolean {
    return this.hubConnection.state === HubConnectionState.Disconnected;
  }

  public static getInstance(): SignalRService {
    if (!this.instance) {
      this.instance = new SignalRService();
    }
    return this.instance;
  }

  public addListener = (messageName: string, callback: (data: any) => void) => {
    const currentCount = this.listenerCounts.get(messageName) || 0;
    if (currentCount === 0) {
      console.log(`Add listener for ${messageName}`);

      this.hubConnection.on(messageName, callback);
    }
    this.listenerCounts.set(messageName, currentCount + 1);
  };

  public removeListener = (messageName: string, callback: (data: any) => void) => {
    const currentCount = this.listenerCounts.get(messageName) || 0;
    if (currentCount > 0) {
      this.listenerCounts.set(messageName, currentCount - 1);
      if (currentCount - 1 === 0) {
        console.log(`Remove listener for ${messageName}`);

        this.hubConnection.off(messageName, callback);
      }
    }
  };

  public async onConnect(): Promise<void> {
    if (this.hubConnection.state !== HubConnectionState.Connected) {
      try {
        await this.hubConnection.start();
        console.log('SignalR connection successfully started');
      } catch (error) {
        console.error('SignalR connection failed to start', error);
      }
    }
  }

  public async onClose(): Promise<void> {
    if (this.hubConnection.state === HubConnectionState.Connected) {
      try {
        await this.hubConnection.stop();
        console.log('SignalR connection successfully closed');
      } catch (error) {
        console.error('Failed to close SignalR connection', error);
      }
    }
  }

  public async invokeHubCommand<T>(methodName: string, argument?: any): Promise<T | undefined> {
    const waitForConnection = async (timeout: number): Promise<boolean> => {
      const startTime = Date.now();
      while (Date.now() - startTime < timeout) {
        if (this.hubConnection.state === 'Connected') {
          return true;
        }
        await new Promise((resolve) => setTimeout(resolve, 100)); // wait for 100ms before checking again
      }
      return false;
    };

    const isConnected = await waitForConnection(3000);
    if (!isConnected) return undefined;

    if (this.hubConnection.state !== 'Connected') return undefined;

    if (isDev && !this.blacklistedMethods.includes(methodName)) {
      if (this.whitelistedMethods.includes(methodName)) {
        console.info('Arguments:', argument);
      }
    }

    try {
      if (!argument) {
        const result = await this.hubConnection.invoke<T>(methodName);
        return result;
      }
      const result = await this.hubConnection.invoke<T>(methodName, argument);
      return result;
    } catch (error: unknown) {
      if (isCSharpException(error)) {
        // store.dispatch(addErrorWithDetail({ Summary: `${methodName} failed`, Detail: error.stack }));
        store.dispatch(addError(`${methodName} failed`));
      } else {
        console.log('Unknown error', error);
        store.dispatch(addError(error as string));
      }

      console.error(`Invocation of method ${methodName} failed`, error);

      throw error;
    }
  }

  private start = () => {
    this.hubConnection
      .start()
      .then(() => {
        this.dispatchEvent(new CustomEvent('signalr_connected'));
        console.log('SignalR connection successfully started');
      })
      .catch((error) => {
        this.hubConnection.off('SendMessage', (message) => {
          console.log(message);
        });
        console.error('SignalR connection failed to start', error);
      });
  };
}

export default SignalRService;
