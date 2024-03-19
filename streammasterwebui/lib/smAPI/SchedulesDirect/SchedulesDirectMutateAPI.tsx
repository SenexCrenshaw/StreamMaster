/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const AddLineup = async (argument: iptv.AddLineup): Promise<void | null> => {
  await invokeHubCommand<void>('AddLineup', argument);
};
export const AddStation = async (argument: iptv.AddStation): Promise<void | null> => {
  await invokeHubCommand<void>('AddStation', argument);
};
export const RemoveLineup = async (argument: iptv.RemoveLineup): Promise<void | null> => {
  await invokeHubCommand<void>('RemoveLineup', argument);
};
export const RemoveStation = async (argument: iptv.RemoveStation): Promise<void | null> => {
  await invokeHubCommand<void>('RemoveStation', argument);
};
