import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,AddLineupRequest,AddStationRequest,RemoveLineupRequest,RemoveStationRequest,CountryData,HeadendDto,LineupPreviewChannel,SubscribedLineup,StationIdLineup,StationChannelName,StationPreview,GetHeadendsRequest,GetLineupPreviewChannelRequest } from '@lib/smAPI/smapiTypes';

export const GetAvailableCountries = async (): Promise<CountryData[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<CountryData[]>('GetAvailableCountries');
};

export const GetHeadends = async (request: GetHeadendsRequest): Promise<HeadendDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<HeadendDto[]>('GetHeadends', request);
};

export const GetLineupPreviewChannel = async (request: GetLineupPreviewChannelRequest): Promise<LineupPreviewChannel[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<LineupPreviewChannel[]>('GetLineupPreviewChannel', request);
};

export const GetLineups = async (): Promise<SubscribedLineup[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SubscribedLineup[]>('GetLineups');
};

export const GetSelectedStationIds = async (): Promise<StationIdLineup[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StationIdLineup[]>('GetSelectedStationIds');
};

export const GetStationChannelNames = async (): Promise<StationChannelName[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StationChannelName[]>('GetStationChannelNames');
};

export const GetStationPreviews = async (): Promise<StationPreview[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<StationPreview[]>('GetStationPreviews');
};

export const AddLineup = async (request: AddLineupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddLineup', request);
};

export const AddStation = async (request: AddStationRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddStation', request);
};

export const RemoveLineup = async (request: RemoveLineupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveLineup', request);
};

export const RemoveStation = async (request: RemoveStationRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveStation', request);
};

