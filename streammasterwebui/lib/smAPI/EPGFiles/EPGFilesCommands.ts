import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,CreateEPGFileFromFormRequest,CreateEPGFileRequest,DeleteEPGFileRequest,ProcessEPGFileRequest,RefreshEPGFileRequest,UpdateEPGFileRequest,EPGFilePreviewDto,EPGFileDto,GetEPGFilePreviewByIdRequest,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetEPGFileNames = async (): Promise<string[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<string[]>('GetEPGFileNames');
};

export const GetEPGFilePreviewById = async (request: GetEPGFilePreviewByIdRequest): Promise<EPGFilePreviewDto[] | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<EPGFilePreviewDto[]>('GetEPGFilePreviewById', request);
};

export const GetEPGFiles = async (): Promise<EPGFileDto[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<EPGFileDto[]>('GetEPGFiles');
};

export const GetEPGNextEPGNumber = async (): Promise<number | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<number>('GetEPGNextEPGNumber');
};

export const GetPagedEPGFiles = async (parameters: QueryStringParameters): Promise<PagedResponse<EPGFileDto> | undefined> => {
  if (isSkipToken(parameters) || parameters === undefined) {
    return undefined;
  }
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

