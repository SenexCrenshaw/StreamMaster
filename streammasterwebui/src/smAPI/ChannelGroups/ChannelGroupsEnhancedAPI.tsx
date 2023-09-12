import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiChannelGroups = iptvApi.enhanceEndpoints({
  endpoints: {
    channelGroupsGetChannelGroup: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupDto) => {
            updateCachedData((draft: iptv.ChannelGroupDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('ChannelGroupsRefresh', (data: iptv.ChannelGroupDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['ChannelGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('ChannelGroupsRefresh');
      }
    },
    channelGroupsGetChannelGroupIdNames: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupIdName[]) => {
            updateCachedData((draft: iptv.ChannelGroupIdName[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('ChannelGroupsRefresh', (data: iptv.ChannelGroupIdName[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['ChannelGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('ChannelGroupsRefresh');
      }
    },
    channelGroupsGetChannelGroups: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.PagedResponseOfChannelGroupDto) => {
            updateCachedData((draft: iptv.PagedResponseOfChannelGroupDto) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('ChannelGroupsRefresh', (data: iptv.PagedResponseOfChannelGroupDto) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['ChannelGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('ChannelGroupsRefresh');
      }
    },
  }
});
