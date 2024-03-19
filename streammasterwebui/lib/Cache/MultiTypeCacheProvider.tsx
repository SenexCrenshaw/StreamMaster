import { IconFileDto, StationChannelName } from '@lib/iptvApi';
import { ReactNode, createContext, useState } from 'react';

interface IconFileDtoCache {
  cache: Map<string, IconFileDto[]>;
  updateCache: (key: string, data: IconFileDto[]) => void;
  removeFromCache: (key: string) => void;
}

interface StationChannelNameCache {
  cache: Map<string, StationChannelName[]>;
  updateCache: (key: string, data: StationChannelName[]) => void;
  removeFromCache: (key: string) => void;
}

export interface MultiTypeCacheContextType {
  iconFileDtoCache: IconFileDtoCache;
  stationChannelNameCache: StationChannelNameCache;
}

// Creating the MultiTypeCacheContext
export const MultiTypeCacheContext = createContext<MultiTypeCacheContextType | null>(null);

/**
 * MultiTypeCacheProvider is a React component that provides multiple caches for different data types.
 * It manages state for each cache and provides functions to update and remove items from these caches.
 */
export const MultiTypeCacheProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  // State for each cache type
  const [iconFileDtoCache, setIconFileDtoCache] = useState<Map<string, IconFileDto[]>>(new Map());
  const [stationChannelNameCache, setStationChannelNameCache] = useState<Map<string, StationChannelName[]>>(new Map());

  // Update functions for each cache type
  const updateIconFileDtoCache = (key: string, data: IconFileDto[]) => {
    setIconFileDtoCache(new Map(iconFileDtoCache.set(key, data)));
  };

  const removeFromIconFileDtoCache = (key: string) => {
    const newCache = new Map(iconFileDtoCache);
    newCache.delete(key);
    setIconFileDtoCache(newCache);
  };

  const updateStationChannelNameCache = (key: string, data: StationChannelName[]) => {
    setStationChannelNameCache(new Map(stationChannelNameCache.set(key, data)));
  };

  const removeFromStationChannelNameCache = (key: string) => {
    const newCache = new Map(stationChannelNameCache);
    newCache.delete(key);
    setStationChannelNameCache(newCache);
  };

  // Context value
  const contextValue = {
    iconFileDtoCache: { cache: iconFileDtoCache, updateCache: updateIconFileDtoCache, removeFromCache: removeFromIconFileDtoCache },
    stationChannelNameCache: { cache: stationChannelNameCache, updateCache: updateStationChannelNameCache, removeFromCache: removeFromStationChannelNameCache }
  };

  return <MultiTypeCacheContext.Provider value={contextValue}>{children}</MultiTypeCacheContext.Provider>;
};
