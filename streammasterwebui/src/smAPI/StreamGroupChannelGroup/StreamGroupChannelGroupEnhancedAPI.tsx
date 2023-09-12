import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiStreamGroupChannelGroup = iptvApi.enhanceEndpoints({
  endpoints: {
    streamGroupChannelGroupGetChannelGroupsFromStreamGroup: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupDto[]) => {
            updateCachedData((draft: iptv.ChannelGroupDto[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('StreamGroupChannelGroupRefresh', (data: iptv.ChannelGroupDto[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['StreamGroupChannelGroup']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('StreamGroupChannelGroupRefresh');
      }
    },
    streamGroupChannelGroupGetAllChannelGroups: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupDto[]) => {
            updateCachedData((draft: iptv.ChannelGroupDto[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('StreamGroupChannelGroupRefresh', (data: iptv.ChannelGroupDto[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['StreamGroupChannelGroup']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('StreamGroupChannelGroupRefresh');
      }
    },
  }
});
