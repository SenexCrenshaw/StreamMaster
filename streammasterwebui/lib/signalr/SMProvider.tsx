import SMLoader from '@components/loader/SMLoader';
import { Logger } from '@lib/common/logger';
import { GetIsSystemReady } from '@lib/smAPI/General/GeneralCommands';
import useGetTaskIsRunning from '@lib/smAPI/General/useGetTaskIsRunning';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import { BlockUI } from 'primereact/blockui';
import React, { ReactNode, createContext, useCallback, useContext, useEffect, useState } from 'react';

interface SMContextState {
  isSystemReady: boolean;
  isTaskRunning: boolean;
  settings: SettingDto;
}

const SMContext = createContext<SMContextState | undefined>(undefined);

interface SMProviderProps {
  children: ReactNode;
}

export const SMProvider: React.FC<SMProviderProps> = ({ children }) => {
  const [isSystemReady, setSystemReady] = useState<boolean>(false);
  const [settings, setSettings] = useState<SettingDto>({} as SettingDto);

  const settingsQuery = useGetSettings();
  // const { data: isSystemReadyQ } = useGetIsSystemReady();
  const { data: isTaskRunning } = useGetTaskIsRunning();

  useEffect(() => {
    if (settingsQuery.data) {
      setSettings(settingsQuery.data);
    }
  }, [settingsQuery.data]);

  const checkSystemReady = useCallback(async () => {
    try {
      const systemReady = await GetIsSystemReady();
      if (systemReady !== isSystemReady) {
        setSystemReady(systemReady ?? false);
        if (systemReady === true && settings.EnableSSL !== undefined) {
          // await DataRefreshAll();
        }
      }
    } catch (error) {
      Logger.error('Error checking system readiness', { error });
      setSystemReady(false);
    }
  }, [isSystemReady, settings.EnableSSL]);

  useEffect(() => {
    checkSystemReady();
    const intervalId = setInterval(checkSystemReady, 1000);
    return () => clearInterval(intervalId);
  }, [checkSystemReady]);

  const contextValue = {
    isSystemReady: isSystemReady === true && settings.EnableSSL !== undefined,
    isTaskRunning: isTaskRunning ?? false,
    settings
  };
  return (
    <SMContext.Provider value={contextValue}>
      {!contextValue.isSystemReady && <SMLoader />}
      <BlockUI blocked={!contextValue.isSystemReady}>{children}</BlockUI>
    </SMContext.Provider>
  );
};

export const useSMContext = (): SMContextState => {
  const context = useContext(SMContext);
  if (!context) {
    throw new Error('useSMContext must be used within a SMProvider');
  }
  return context;
};
