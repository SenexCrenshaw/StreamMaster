/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubCommand } from '@lib/signalr/signalr';

export const EpgSync = async (): Promise<void | null> => {
  await invokeHubCommand<void>('EpgSync');
};
