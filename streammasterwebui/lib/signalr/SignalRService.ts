import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { baseHostURL, isDev } from '@lib/settings';

class SignalRService extends EventTarget {
  private static instance: SignalRService;
  public hubConnection: HubConnection;
  private blacklistedMethods: string[] = ['GetLog', 'GetIconFromSource'];
  private whitelistedMethods: string[] = [];
  private listenerCounts: Map<string, number> = new Map<string, number>();

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

  private constructor() {
    super();

    const url = `${baseHostURL}/streammasterhub`;
    this.hubConnection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Error)
      .withUrl(url)
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          if (retryContext.elapsedMilliseconds < 60_000) {
            return 2000;
          }
          return 2000;
        }
      })
      .build();

    this.hubConnection.onclose(() => {
      this.dispatchEvent(new CustomEvent('signalr_disconnected'));
      console.log('SignalR connection closed');
    });

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

    this.hubConnection.onclose((callback) => {
      console.log('SignalR connection closed', callback);
    });
  }

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

  public static getInstance(): SignalRService {
    if (!this.instance) {
      this.instance = new SignalRService();
    }
    return this.instance;
  }

  public get isConnected(): boolean {
    return this.hubConnection.state === HubConnectionState.Connected;
  }

  public get isDisConnected(): boolean {
    return this.hubConnection.state === HubConnectionState.Disconnected;
  }

  public async invokeHubCommand<T>(methodName: string, argument?: any): Promise<T | null> {
    if (!this.isConnected) {
      console.warn('SignalR connection is not established');
      return null;
    }

    if (isDev && !this.blacklistedMethods.includes(methodName)) {
      if (this.whitelistedMethods.includes(methodName)) {
        console.info('Arguments:', argument);
      }
    }

    try {
      const result = await this.hubConnection.invoke<T>(methodName, argument);
      return result;
    } catch (error) {
      console.error(`Invocation of method ${methodName} failed`, error);
      return null;
    }
  }
}

export default SignalRService;