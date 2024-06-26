import React, { ReactNode, createContext, useCallback, useContext, useState } from 'react';

// Define the shape of the context
interface SMButtonContextProps {
  buttonDisabled: boolean;
  setDisabled: () => void;
  setEnabled: () => void;
  toggleEnabled: () => void;
}

// Create the context with default values
const SMButtonContext = createContext<SMButtonContextProps | undefined>(undefined);

// Create a provider component
export const SMButtonProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [buttonDisabled, setButtonDisabled] = useState<boolean>(false);

  const setDisabled = useCallback(() => {
    setButtonDisabled(true);
  }, []);

  const setEnabled = useCallback(() => {
    setButtonDisabled(false);
  }, []);

  const toggleEnabled = useCallback(() => {
    setButtonDisabled(!buttonDisabled);
  }, [buttonDisabled]);

  const contextValue: SMButtonContextProps = {
    buttonDisabled,
    setDisabled,
    setEnabled,
    toggleEnabled
  };

  return <SMButtonContext.Provider value={contextValue}>{children}</SMButtonContext.Provider>;
};

// Create a custom hook to use the context
export const useSMButtonContext = (): SMButtonContextProps => {
  const context = useContext(SMButtonContext);
  if (!context) {
    throw new Error('useSMButtonContext must be used within an SMButtonProvider');
  }
  return context;
};
