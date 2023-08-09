import { hubConnection } from "../app/signalr";
import {

  type UpdateSettingRequest,
} from "./iptvApi";

export const UpdateSetting = async (arg: UpdateSettingRequest) => {
  const data = await hubConnection.invoke('UpdateSetting', arg);

  return data;
};
