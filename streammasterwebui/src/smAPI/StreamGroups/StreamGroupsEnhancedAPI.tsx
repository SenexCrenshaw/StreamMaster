import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import type * as iptv from '../../store/iptvApi';
import { iptvApi } from '../../store/iptvApi';

export const enhancedApiStreamGroups = iptvApi.enhanceEndpoints({
  endpoints: {
    streamGroupsGetStreamGroup: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StreamGroupDto) => {
            updateCachedData((draft: iptv.StreamGroupDto) => {
              draft = data
              return draft;
            });
          };

          hubConnection.off('StreamGroupsRefresh');
          hubConnection.on('StreamGroupsRefresh', (data: iptv.StreamGroupDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['StreamGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    streamGroupsGetStreamGroupEpgForGuide: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.EpgGuide) => {
            updateCachedData((draft: iptv.EpgGuide) => {
              draft = data
              return draft;
            });
          };

          hubConnection.off('StreamGroupsRefresh');
          hubConnection.on('StreamGroupsRefresh', (data: iptv.EpgGuide) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['StreamGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    streamGroupsGetStreamGroups: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StreamGroupDto[]) => {
            updateCachedData((draft: iptv.PagedResponseOfStreamGroupDto) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          hubConnection.off('StreamGroupsRefresh');
          hubConnection.on('StreamGroupsRefresh', (data: iptv.StreamGroupDto[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['StreamGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
