// CombinedContext.tsx
import React, { ReactNode } from 'react';
import { SMButtonProvider, useSMButtonContext } from './SMButtonContext';
import { SMOverlayProvider, useSMOverlay } from './SMOverlayContext';

// Create a combined context provider
export const CombinedProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  return (
    <SMOverlayProvider>
      <SMButtonProvider>{children}</SMButtonProvider>
    </SMOverlayProvider>
  );
};

// Create a custom hook to use both contexts
export const useCombinedContext = () => {
  const overlayContext = useSMOverlay();
  const buttonContext = useSMButtonContext();

  // Merge both contexts
  return {
    ...overlayContext,
    ...buttonContext
  };
};
