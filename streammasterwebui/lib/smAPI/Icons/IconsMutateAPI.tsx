import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const AutoMatchIconToStreams = async (arg: iptv.AutoMatchIconToStreamsRequest): Promise<void> => {
  if (isDebug) console.log('AutoMatchIconToStreams');
  await hubConnection.invoke('AutoMatchIconToStreams', arg);
};

