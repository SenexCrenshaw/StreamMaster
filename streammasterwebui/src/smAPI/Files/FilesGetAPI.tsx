import { hubConnection } from "../../app/signalr";

export const GetFile = async (arg: string): Promise<void> => {
  await hubConnection.invoke('GetFile', arg);
};

