import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,AddHeadendToViewRequest,AddLineupRequest,AddStationRequest,RemoveHeadendToViewRequest,RemoveLineupRequest,RemoveStationRequest,CountryData,HeadendDto,HeadendToView,LineupPreviewChannel,StationIdLineup,StationChannelName,StationPreview,SubscribedLineup,GetHeadendsByCountryPostalRequest,GetLineupPreviewChannelRequest } from '@lib/smAPI/smapiTypes';

export const GetAvailableCountries = async (): Promise<CountryData[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<CountryData[]>('GetAvailableCountries');
};

export const GetHeadendsByCountryPostal = async (request: GetHeadendsByCountryPostalRequest): Promise<HeadendDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<HeadendDto[]>('GetHeadendsByCountryPostal', request);
};

export const GetHeadendsToView = async (): Promise<HeadendToView[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<HeadendToView[]>('GetHeadendsToView');
};

export const GetLineupPreviewChannel = async (request: GetLineupPreviewChannelRequest): Promise<LineupPreviewChannel[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<LineupPreviewChannel[]>('GetLineupPreviewChannel', request);
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

export const GetSubScribedHeadends = async (): Promise<HeadendDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<HeadendDto[]>('GetSubScribedHeadends');
};

export const GetSubscribedLineups = async (): Promise<SubscribedLineup[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SubscribedLineup[]>('GetSubscribedLineups');
};

export const AddHeadendToView = async (request: AddHeadendToViewRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddHeadendToView', request);
};

export const AddLineup = async (request: AddLineupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddLineup', request);
};

export const AddStation = async (request: AddStationRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AddStation', request);
};

export const RemoveHeadendToView = async (request: RemoveHeadendToViewRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveHeadendToView', request);
};

export const RemoveLineup = async (request: RemoveLineupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveLineup', request);
};

export const RemoveStation = async (request: RemoveStationRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RemoveStation', request);
};

