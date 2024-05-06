import SMLoader from '@components/loader/SMLoader';
import { BlockUI } from 'primereact/blockui';
import React, { ReactNode, createContext, useContext, useState } from 'react';

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
