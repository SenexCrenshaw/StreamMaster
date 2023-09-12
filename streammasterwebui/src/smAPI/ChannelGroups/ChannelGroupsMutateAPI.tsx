import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const CreateChannelGroup = async (arg: iptv.CreateChannelGroupRequest): Promise<void> => {
  await hubConnection.invoke('CreateChannelGroup', arg);
};

export const DeleteAllChannelGroupsFromParameters = async (arg: iptv.DeleteAllChannelGroupsFromParametersRequest): Promise<void> => {
  await hubConnection.invoke('DeleteAllChannelGroupsFromParameters', arg);
};

export const DeleteChannelGroup = async (arg: iptv.DeleteChannelGroupRequest): Promise<void> => {
  await hubConnection.invoke('DeleteChannelGroup', arg);
};

export const UpdateChannelGroup = async (arg: iptv.UpdateChannelGroupRequest): Promise<void> => {
  await hubConnection.invoke('UpdateChannelGroup', arg);
};

export const UpdateChannelGroups = async (arg: iptv.UpdateChannelGroupsRequest): Promise<void> => {
  await hubConnection.invoke('UpdateChannelGroups', arg);
};

