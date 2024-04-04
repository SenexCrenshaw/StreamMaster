import SignalRService from '@lib/signalr/SignalRService';
import { EPGFileDto,CreateEPGFileRequest,DeleteEPGFileRequest,ProcessEPGFileRequest,RefreshEPGFileRequest,UpdateEPGFileRequest,EPGColorDto,EPGFilePreviewDto,GetEPGFilePreviewByIdRequest } from '@lib/smAPI/smapiTypes';

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

export const CreateEPGFile = async (request: CreateEPGFileRequest): Promise<EPGFileDto | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<EPGFileDto>('CreateEPGFile', request);
};

export const DeleteEPGFile = async (request: DeleteEPGFileRequest): Promise<number[] | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<number[]>('DeleteEPGFile', request);
};

export const ProcessEPGFile = async (request: ProcessEPGFileRequest): Promise<EPGFileDto | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<EPGFileDto>('ProcessEPGFile', request);
};

export const RefreshEPGFile = async (request: RefreshEPGFileRequest): Promise<EPGFileDto | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<EPGFileDto>('RefreshEPGFile', request);
};

export const UpdateEPGFile = async (request: UpdateEPGFileRequest): Promise<EPGFileDto | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<EPGFileDto>('UpdateEPGFile', request);
};

