import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const UpdateSetting = async (arg: iptv.UpdateSettingRequest): Promise<void> => {
  if (isDebug) console.log('UpdateSetting');
  await hubConnection.invoke('UpdateSetting', arg);
};

