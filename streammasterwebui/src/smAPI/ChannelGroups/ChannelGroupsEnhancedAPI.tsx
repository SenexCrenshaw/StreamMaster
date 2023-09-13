import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import type * as iptv from '../../store/iptvApi';
import { iptvApi } from '../../store/iptvApi';

export const enhancedApiChannelGroups = iptvApi.enhanceEndpoints({
  endpoints: {
    channelGroupsGetChannelGroup: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupDto) => {
            updateCachedData((draft: iptv.ChannelGroupDto) => {
              draft = data
              return draft;
            });
          };

          // hubConnection.off('ChannelGroupsRefresh');
          hubConnection.on('ChannelGroupsRefresh', (data: iptv.ChannelGroupDto) => {
            console.log('channelGroupsGetChannelGroup ChannelGroupsRefresh')
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
      }
    },
    channelGroupsGetChannelGroupIdNames: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupIdName[]) => {
            updateCachedData((draft: iptv.ChannelGroupIdName[]) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          // hubConnection.off('ChannelGroupsRefresh');
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
      }
    },
    channelGroupsGetChannelGroups: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupDto[]) => {
            updateCachedData((draft: iptv.PagedResponseOfChannelGroupDto) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          // hubConnection.off('ChannelGroupsRefresh');
          hubConnection.on('ChannelGroupsRefresh', (data: iptv.ChannelGroupDto[]) => {
            if (isEmptyObject(data)) {
              console.log('ChannelGroupsRefresh: invalidateTags ChannelGroups')
              dispatch(iptvApi.util.invalidateTags(['ChannelGroups']));
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
