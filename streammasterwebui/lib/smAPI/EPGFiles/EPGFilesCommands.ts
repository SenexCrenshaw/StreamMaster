import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,CreateEPGFileFromFormRequest,CreateEPGFileRequest,DeleteEPGFileRequest,ProcessEPGFileRequest,RefreshEPGFileRequest,UpdateEPGFileRequest,EPGColorDto,EPGFilePreviewDto,EPGFileDto,GetEPGFilePreviewByIdRequest,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetEPGColors = async (): Promise<EPGColorDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<EPGColorDto[]>('GetEPGColors');
};

export const GetEPGFilePreviewById = async (request: GetEPGFilePreviewByIdRequest): Promise<EPGFilePreviewDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<EPGFilePreviewDto[]>('GetEPGFilePreviewById', request);
};

export const GetEPGNextEPGNumber = async (): Promise<number | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<number>('GetEPGNextEPGNumber');
};

export const GetPagedEPGFiles = async (parameters: QueryStringParameters): Promise<PagedResponse<EPGFileDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<EPGFileDto>>('GetPagedEPGFiles', parameters);
};

export const CreateEPGFileFromForm = async (request: CreateEPGFileFromFormRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateEPGFileFromForm', request);
};

export const CreateEPGFile = async (request: CreateEPGFileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateEPGFile', request);
};

export const DeleteEPGFile = async (request: DeleteEPGFileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('DeleteEPGFile', request);
};

export const ProcessEPGFile = async (request: ProcessEPGFileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('ProcessEPGFile', request);
};

export const RefreshEPGFile = async (request: RefreshEPGFileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RefreshEPGFile', request);
};

export const UpdateEPGFile = async (request: UpdateEPGFileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateEPGFile', request);
};

