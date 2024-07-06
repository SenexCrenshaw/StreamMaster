import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { UpdateSettingResponse,UpdateSettingRequest,SettingDto } from '@lib/smAPI/smapiTypes';

export const GetSettings = async (): Promise<SettingDto | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SettingDto>('GetSettings');
};

export const UpdateSetting = async (request: UpdateSettingRequest): Promise<UpdateSettingResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<UpdateSettingResponse>('UpdateSetting', request);
};

