import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const CreateStreamGroup = async (arg: iptv.CreateStreamGroupRequest): Promise<void> => {
  await hubConnection.invoke('CreateStreamGroup', arg);
};

export const DeleteStreamGroup = async (arg: iptv.DeleteStreamGroupRequest): Promise<void> => {
  await hubConnection.invoke('DeleteStreamGroup', arg);
};

export const UpdateStreamGroup = async (arg: iptv.UpdateStreamGroupRequest): Promise<void> => {
  await hubConnection.invoke('UpdateStreamGroup', arg);
};

