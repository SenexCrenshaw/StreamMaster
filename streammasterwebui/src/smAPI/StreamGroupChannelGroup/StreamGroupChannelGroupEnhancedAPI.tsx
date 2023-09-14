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
            updateCachedData((draft: iptv.StreamGroupChannelGroupGetChannelGroupsFromStreamGroupApiResponse) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          const doStreamGroupChannelGroupGetChannelGroupsFromStreamGroupUpdate = (data: iptv.ChannelGroupDto[]) => {
            // console.log('doStreamGroupChannelGroupGetChannelGroupsFromStreamGroupUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['StreamGroupChannelGroup']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('StreamGroupChannelGroupRefresh', doStreamGroupChannelGroupGetChannelGroupsFromStreamGroupUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
