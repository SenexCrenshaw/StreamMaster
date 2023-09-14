import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiEpgFiles = iptvApi.enhanceEndpoints({
  endpoints: {
    epgFilesGetEpgFile: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.EpgFileDto) => {
            updateCachedData((draft: iptv.EpgFilesGetEpgFileApiResponse) => {
              draft=data
              return draft;
            });
          };

          const doEpgFilesGetEpgFileUpdate = (data: iptv.EpgFileDto) => {
            // console.log('doEpgFilesGetEpgFileUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['EPGFiles']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('EPGFilesRefresh', doEpgFilesGetEpgFileUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    epgFilesGetPagedEpgFiles: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.EpgFileDto[]) => {
            updateCachedData((draft: iptv.EpgFilesGetPagedEpgFilesApiResponse) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          const doEpgFilesGetPagedEpgFilesUpdate = (data: iptv.EpgFileDto[]) => {
            // console.log('doEpgFilesGetPagedEpgFilesUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['EPGFiles']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('EPGFilesRefresh', doEpgFilesGetPagedEpgFilesUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
