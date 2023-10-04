import type * as iptv from '@/lib/iptvApi'
import { iptvApi } from '@/lib/iptvApi'
import { hubConnection } from '@/lib/signalr/signalr'
import { isEmptyObject } from '../common/common'

export const enhancedApiChannelGroupsLocal = iptvApi.enhanceEndpoints({
  endpoints: {
    channelGroupsGetChannelGroup: {
      async onCacheEntryAdded(
        api,
        { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved },
      ) {
        try {
          await cacheDataLoaded

          const updateCachedDataWithResults = (
            data: iptv.ChannelGroupStreamCount,
          ) => {
            updateCachedData((draft: iptv.ChannelGroupDto) => {
              draft.activeCount = data.activeCount
              draft.totalCount = data.totalCount
              draft.hiddenCount = data.hiddenCount
              return draft
            })
          }

          const doUpdates = (data: iptv.ChannelGroupStreamCount) => {
            console.log(
              'UpdateChannelGroupVideoStreamCounts ChannelGroupsRefresh',
            )
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['ChannelGroups']))
            } else {
              updateCachedDataWithResults(data)
            }
          }

          // hubConnection.off('UpdateChannelGroupVideoStreamCounts', doUpdates);
          hubConnection.on('UpdateChannelGroupVideoStreamCounts', doUpdates)
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error)
        }

        await cacheEntryRemoved
      },
    },
    // channelGroupsGetChannelGroups: {
    //   async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
    //     try {
    //       await cacheDataLoaded;

    //       const updateCachedDataWithResults = (data: iptv.ChannelGroupStreamCount[]) => {
    //         updateCachedData((draft: iptv.PagedResponseOfChannelGroupDto) => {

    //           data.forEach(item => {
    //             const index = draft.data.findIndex(existingItem => existingItem.id === item.channelGroupId);
    //             if (index !== -1) {
    //               draft.data[index].activeCount = item.activeCount;
    //               draft.data[index].totalCount = item.totalCount;
    //               draft.data[index].hiddenCount = item.hiddenCount;
    //             }
    //           });

    //           return draft;
    //         });
    //       };

    //       // hubConnection.off('ChannelGroupsRefresh');
    //       hubConnection.on('UpdateChannelGroupVideoStreamCounts', (data: iptv.ChannelGroupStreamCount[]) => {
    //         if (isEmptyObject(data)) {
    //           console.log('ChannelGroupsRefresh: invalidateTags ChannelGroups')
    //           dispatch(iptvApi.util.invalidateTags(['ChannelGroups']));
    //         } else {
    //           updateCachedDataWithResults(data);
    //         }
    //       });

    //     } catch (error) {
    //       console.error('Error in onCacheEntryAdded:', error);
    //     }

    //     await cacheEntryRemoved;
    //   }
    // },
  },
})
