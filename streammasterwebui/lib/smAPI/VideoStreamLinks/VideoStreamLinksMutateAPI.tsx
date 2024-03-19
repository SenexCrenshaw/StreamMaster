/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const AddVideoStreamToVideoStream = async (argument: iptv.AddVideoStreamToVideoStreamRequest): Promise<void | null> => {
  await invokeHubCommand<void>('AddVideoStreamToVideoStream', argument);
};
export const RemoveVideoStreamFromVideoStream = async (argument: iptv.RemoveVideoStreamFromVideoStreamRequest): Promise<void | null> => {
  await invokeHubCommand<void>('RemoveVideoStreamFromVideoStream', argument);
};
