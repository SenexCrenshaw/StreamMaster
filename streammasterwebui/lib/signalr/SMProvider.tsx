import SMLoader from '@components/loader/SMLoader';
import { DataRefreshAll } from '@lib/smAPI/DataRefreshAll';

import { Logger } from '@lib/common/logger';

import { GetIsSystemReady } from '@lib/smAPI/General/GeneralCommands';
import useGetTaskIsRunning from '@lib/smAPI/General/useGetTaskIsRunning';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import { BlockUI } from 'primereact/blockui';
import React, { ReactNode, createContext, useContext, useEffect, useState } from 'react';

interface SMContextState {
  isSystemReady: boolean;
  isTaskRunning: boolean;
  setSystemReady: React.Dispatch<React.SetStateAction<boolean>>;
  settings: SettingDto;
  setSettings: React.Dispatch<React.SetStateAction<SettingDto>>;
}

const SMContext = createContext<SMContextState | undefined>(undefined);

interface SMProviderProps {
  children: ReactNode;
}

export const SMProvider: React.FC<SMProviderProps> = ({ children }) => {
  const [isSystemReady, setSystemReady] = useState<boolean>(false);
  const [settings, setSettings] = useState<SettingDto>({} as SettingDto);
  const settingsQuery = useGetSettings();
  const { data: isTaskRunning } = useGetTaskIsRunning();

  useEffect(() => {
    if (settingsQuery.data) {
      setSettings(settingsQuery.data);
    }
  }, [settingsQuery.data]);

  const value = {
    isSystemReady: isSystemReady && settingsQuery.data !== undefined,
    isTaskRunning: isTaskRunning ?? false,
    setSettings,
    setSystemReady,
    settings
  };

  useEffect(() => {
    const checkSystemReady = async () => {
      try {
        const result = await GetIsSystemReady();
        if (result !== isSystemReady) {
          setSystemReady(result ?? false);
          if (result === true && settingsQuery.data !== undefined) {
            await DataRefreshAll();
          }
        }
      } catch (error) {
        Logger.error('Error checking system readiness', { error });
        setSystemReady(false);
      }
    };

    const intervalId = setInterval(checkSystemReady, 1000);

    return () => clearInterval(intervalId);
  }, [isSystemReady, settingsQuery.data]);

  // return (
  //   <SMContext.Provider value={value}>
  //     {value.isSystemReady && <SMLoader />}
  //     <BlockUI blocked={value.isSystemReady}>{children}</BlockUI>
  //   </SMContext.Provider>
  // );

  return (
    <SMContext.Provider value={value}>
      {value.isSystemReady !== true && <SMLoader />}
      <BlockUI blocked={value.isSystemReady !== true}>{children}</BlockUI>
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
