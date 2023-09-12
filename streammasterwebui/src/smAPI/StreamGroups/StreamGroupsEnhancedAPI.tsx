import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiStreamGroups = iptvApi.enhanceEndpoints({
  endpoints: {
    streamGroupsGetStreamGroup: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StreamGroupDto) => {
            updateCachedData((draft: iptv.StreamGroupDto) => {
              draft=data
              return draft;
            });
          };

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
        hubConnection.off('StreamGroupsRefresh');
      }
    },
    streamGroupsGetStreamGroupEpgForGuide: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.EpgGuide) => {
            updateCachedData((draft: iptv.EpgGuide) => {
              draft=data
              return draft;
            });
          };

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
        hubConnection.off('StreamGroupsRefresh');
      }
    },
    streamGroupsGetStreamGroups: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.PagedResponseOfStreamGroupDto) => {
            updateCachedData((draft: iptv.PagedResponseOfStreamGroupDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('StreamGroupsRefresh', (data: iptv.PagedResponseOfStreamGroupDto) => {
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
        hubConnection.off('StreamGroupsRefresh');
      }
    },
  }
});
