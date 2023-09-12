import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiProgrammes = iptvApi.enhanceEndpoints({
  endpoints: {
    programmesGetProgramme: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.Programme[]) => {
            updateCachedData((draft: iptv.Programme[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('ProgrammesRefresh', (data: iptv.Programme[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Programmes']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('ProgrammesRefresh');
      }
    },
    programmesGetProgrammeChannels: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ProgrammeChannel[]) => {
            updateCachedData((draft: iptv.ProgrammeChannel[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('ProgrammesRefresh', (data: iptv.ProgrammeChannel[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Programmes']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('ProgrammesRefresh');
      }
    },
    programmesGetProgrammeNameSelections: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.PagedResponseOfProgrammeNameDto) => {
            updateCachedData((draft: iptv.PagedResponseOfProgrammeNameDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('ProgrammesRefresh', (data: iptv.PagedResponseOfProgrammeNameDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Programmes']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('ProgrammesRefresh');
      }
    },
    programmesGetProgrammes: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.Programme[]) => {
            updateCachedData((draft: iptv.Programme[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('ProgrammesRefresh', (data: iptv.Programme[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Programmes']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('ProgrammesRefresh');
      }
    },
    programmesGetProgrammsSimpleQuery: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ProgrammeNameDto[]) => {
            updateCachedData((draft: iptv.ProgrammeNameDto[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('ProgrammesRefresh', (data: iptv.ProgrammeNameDto[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Programmes']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('ProgrammesRefresh');
      }
    },
    programmesGetProgrammeFromDisplayName: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ProgrammeNameDto) => {
            updateCachedData((draft: iptv.ProgrammeNameDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('ProgrammesRefresh', (data: iptv.ProgrammeNameDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Programmes']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('ProgrammesRefresh');
      }
    },
  }
});
