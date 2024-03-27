import {SettingDto,SDSystemStatus} from '@lib/apiDefs';
import SignalRService from '@lib/signalr/SignalRService';

export const GetSettings = async (): Promise<any | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SettingDto>('GetSettings');
};

export const GetSystemStatus = async (): Promise<any | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SDSystemStatus>('GetSystemStatus');
};

