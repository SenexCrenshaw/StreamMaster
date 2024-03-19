/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const AutoMatchIconToStreams = async (argument: iptv.AutoMatchIconToStreamsRequest): Promise<void | null> => {
  await invokeHubCommand<void>('AutoMatchIconToStreams', argument);
};
