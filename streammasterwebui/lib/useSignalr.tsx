'use client'

/* Core */
import { useSignalRConnection } from './signalr/useSignalRConnection';

export const useSignalr = (props: React.PropsWithChildren) => {
  useSignalRConnection();

  return props.children;
}
