import SMLoader from '@components/loader/SMLoader';
import { DataRefreshAll } from '@lib/smAPI/DataRefreshAll';
import { GetIsSystemReady } from '@lib/smAPI/Settings/SettingsCommands';
import { BlockUI } from 'primereact/blockui';
import React, { ReactNode, createContext, useContext, useEffect, useState } from 'react';

interface SMContextState {
  isSystemReady: boolean;
  setSystemReady: React.Dispatch<React.SetStateAction<boolean>>;
}

const SMContext = createContext<SMContextState | undefined>(undefined);

interface SMProviderProps {
  children: ReactNode;
}

export const SMProvider: React.FC<SMProviderProps> = ({ children }) => {
  const [isSystemReady, setSystemReady] = useState<boolean>(false);

  const value = { isSystemReady, setSystemReady };

  useEffect(() => {
    const intervalId = setInterval(() => {
      GetIsSystemReady()
        .then((result) => {
          if (result !== isSystemReady) {
            setSystemReady(result ?? false);
            if (result === true && isSystemReady === false) {
              DataRefreshAll();
            }
          }
        })
        .catch((e) => {
          console.error(e);
          setSystemReady(false);
        });
    }, 1000);

    return () => clearInterval(intervalId);
  }, [isSystemReady]);

  return (
    <SMContext.Provider value={value}>
      {
        <>
          {!isSystemReady && <SMLoader />}
          <BlockUI blocked={!isSystemReady}>{children}</BlockUI>
        </>
      }
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
