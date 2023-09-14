import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiProgrammes = iptvApi.enhanceEndpoints({
  endpoints: {
    programmesGetPagedProgrammeNameSelections: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ProgrammeNameDto[]) => {
            updateCachedData((draft: iptv.ProgrammesGetPagedProgrammeNameSelectionsApiResponse) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          const doProgrammesGetPagedProgrammeNameSelectionsUpdate = (data: iptv.ProgrammeNameDto[]) => {
            // console.log('doProgrammesGetPagedProgrammeNameSelectionsUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Programmes']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('ProgrammesRefresh', doProgrammesGetPagedProgrammeNameSelectionsUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    programmesGetProgrammsSimpleQuery: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ProgrammeNameDto[]) => {
            updateCachedData((draft: iptv.ProgrammesGetProgrammsSimpleQueryApiResponse) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          const doProgrammesGetProgrammsSimpleQueryUpdate = (data: iptv.ProgrammeNameDto[]) => {
            // console.log('doProgrammesGetProgrammsSimpleQueryUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['Programmes']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('ProgrammesRefresh', doProgrammesGetProgrammsSimpleQueryUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
