import { hubConnection } from "../app/signalr";
import {
  type UpdateSettingResponse,
  type UpdateSettingRequest,
} from "./iptvApi";

export const UpdateSetting = async (arg: UpdateSettingRequest): Promise<UpdateSettingResponse> => {
  const data = await hubConnection.invoke('UpdateSetting', arg);

  return data;
};
