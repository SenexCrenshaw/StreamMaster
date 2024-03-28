import {SettingDto,SDSystemStatus} from '@lib/smAPI/smapiTypes';
import SignalRService from '@lib/signalr/SignalRService';

export const GetSettings = async (): Promise<any | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SettingDto>('GetSettings');
};

export const GetSystemStatus = async (): Promise<any | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SDSystemStatus>('GetSystemStatus');
};

