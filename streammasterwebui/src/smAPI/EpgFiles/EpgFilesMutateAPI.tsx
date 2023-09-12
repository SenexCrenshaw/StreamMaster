import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const CreateEpgFile = async (arg: iptv.CreateEpgFileRequest): Promise<void> => {
  await hubConnection.invoke('CreateEpgFile', arg);
};

export const CreateEpgFileFromForm = async (): Promise<void> => {
  await hubConnection.invoke('CreateEpgFileFromForm');
};

export const DeleteEpgFile = async (arg: iptv.DeleteEpgFileRequest): Promise<void> => {
  await hubConnection.invoke('DeleteEpgFile', arg);
};

export const ProcessEpgFile = async (arg: iptv.ProcessEpgFileRequest): Promise<void> => {
  await hubConnection.invoke('ProcessEpgFile', arg);
};

export const RefreshEpgFile = async (arg: iptv.RefreshEpgFileRequest): Promise<void> => {
  await hubConnection.invoke('RefreshEpgFile', arg);
};

export const ScanDirectoryForEpgFiles = async (): Promise<void> => {
  await hubConnection.invoke('ScanDirectoryForEpgFiles');
};

export const UpdateEpgFile = async (arg: iptv.UpdateEpgFileRequest): Promise<void> => {
  await hubConnection.invoke('UpdateEpgFile', arg);
};

