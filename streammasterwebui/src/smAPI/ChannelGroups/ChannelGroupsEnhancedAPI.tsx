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
            updateCachedData((draft: iptv.ChannelGroupsGetChannelGroupApiResponse) => {
              draft = data
              return draft;
            });
          };

          const doChannelGroupsGetChannelGroupUpdate = (data: iptv.ChannelGroupDto) => {
            // console.log('doChannelGroupsGetChannelGroupUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['ChannelGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('ChannelGroupsRefresh', doChannelGroupsGetChannelGroupUpdate);

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
            updateCachedData((draft: iptv.ChannelGroupsGetChannelGroupIdNamesApiResponse) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          const doChannelGroupsGetChannelGroupIdNamesUpdate = (data: iptv.ChannelGroupIdName[]) => {
            // console.log('doChannelGroupsGetChannelGroupIdNamesUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['ChannelGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('ChannelGroupsRefresh', doChannelGroupsGetChannelGroupIdNamesUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    channelGroupsGetChannelGroupsForStreamGroup: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupDto[]) => {
            updateCachedData((draft: iptv.ChannelGroupsGetChannelGroupsForStreamGroupApiResponse) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          const doChannelGroupsGetChannelGroupsForStreamGroupUpdate = (data: iptv.ChannelGroupDto[]) => {
            // console.log('doChannelGroupsGetChannelGroupsForStreamGroupUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['ChannelGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('ChannelGroupsRefresh', doChannelGroupsGetChannelGroupsForStreamGroupUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    channelGroupsGetPagedChannelGroups: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupDto[]) => {
            updateCachedData((draft: iptv.ChannelGroupsGetPagedChannelGroupsApiResponse) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          const doChannelGroupsGetPagedChannelGroupsUpdate = (data: iptv.ChannelGroupDto[]) => {
            // console.log('doChannelGroupsGetPagedChannelGroupsUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['ChannelGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('ChannelGroupsRefresh', doChannelGroupsGetPagedChannelGroupsUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
