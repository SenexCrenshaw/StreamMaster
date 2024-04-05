import SignalRService from '@lib/signalr/SignalRService';
import { DefaultAPIResponse,CreateEPGFileRequest,DeleteEPGFileRequest,ProcessEPGFileRequest,RefreshEPGFileRequest,UpdateEPGFileRequest,EPGColorDto,EPGFilePreviewDto,GetEPGFilePreviewByIdRequest } from '@lib/smAPI/smapiTypes';

export const GetEPGColors = async (): Promise<EPGColorDto[] | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<EPGColorDto[]>('GetEPGColors');
};

export const GetEPGFilePreviewById = async (request: GetEPGFilePreviewByIdRequest): Promise<EPGFilePreviewDto[] | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<EPGFilePreviewDto[]>('GetEPGFilePreviewById', request);
};

export const GetEPGNextEPGNumber = async (): Promise<number | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<number>('GetEPGNextEPGNumber');
};

export const CreateEPGFile = async (request: CreateEPGFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('CreateEPGFile', request);
};

export const DeleteEPGFile = async (request: DeleteEPGFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('DeleteEPGFile', request);
};

export const ProcessEPGFile = async (request: ProcessEPGFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('ProcessEPGFile', request);
};

export const RefreshEPGFile = async (request: RefreshEPGFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('RefreshEPGFile', request);
};

export const UpdateEPGFile = async (request: UpdateEPGFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('UpdateEPGFile', request);
};

