import { isDebug } from '@/lib/settings';
import { hubConnection } from '@/lib/signalr/signalr';


export const GetFile = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetFile');
  await hubConnection.invoke('GetFile', arg);
};

