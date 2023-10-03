/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const AddVideoStreamToVideoStream = async (arg: iptv.AddVideoStreamToVideoStreamRequest): Promise<void> => {
  if (isDebug) console.log('AddVideoStreamToVideoStream');
  await hubConnection.invoke('AddVideoStreamToVideoStream', arg);
};

export const RemoveVideoStreamFromVideoStream = async (arg: iptv.RemoveVideoStreamFromVideoStreamRequest): Promise<void> => {
  if (isDebug) console.log('RemoveVideoStreamFromVideoStream');
  await hubConnection.invoke('RemoveVideoStreamFromVideoStream', arg);
};

