import SignalRService from '@lib/signalr/SignalRService';
import { SettingDto,SDSystemStatus } from '@lib/smAPI/smapiTypes';

export const GetIsSystemReady = async (): Promise<boolean | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<boolean>('GetIsSystemReady');
};

export const GetSettings = async (): Promise<SettingDto | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SettingDto>('GetSettings');
};

export const GetSystemStatus = async (): Promise<SDSystemStatus | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SDSystemStatus>('GetSystemStatus');
};

