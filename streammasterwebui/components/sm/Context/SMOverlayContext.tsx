// SMOverlayContext.tsx
import React, { ReactNode, createContext, useCallback, useContext, useState } from 'react';

// Define the shape of the context
interface SMOverlayContextProps {
  isOpen: boolean;
  toggleOpen: () => void;
  closeOverlay: () => void;
}

// Create the context with default values
const SMOverlayContext = createContext<SMOverlayContextProps | undefined>(undefined);

// Create a provider component
export const SMOverlayProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [isOpen, setIsOpen] = useState(false);

  const toggleOpen = useCallback(() => setIsOpen((prev) => !prev), []);
  const closeOverlay = useCallback(() => setIsOpen(false), []);

  return <SMOverlayContext.Provider value={{ closeOverlay, isOpen, toggleOpen }}>{children}</SMOverlayContext.Provider>;
};

// Create a custom hook to use the context
export const useSMOverlay = (): SMOverlayContextProps => {
  const context = useContext(SMOverlayContext);
  if (!context) {
    throw new Error('useSMOverlay must be used within an SMOverlayProvider');
  }
  return context;
};
