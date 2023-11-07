/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const AddLineup = async (argument: iptv.AddLineup): Promise<void | null> => {
  await invokeHubConnection<void>('AddLineup', argument);
};
export const DeleteLineup = async (argument: iptv.DeleteLineup): Promise<void | null> => {
  await invokeHubConnection<void>('DeleteLineup', argument);
};
