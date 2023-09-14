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
            updateCachedData((draft: iptv.StreamGroupsGetStreamGroupApiResponse) => {
              draft=data
              return draft;
            });
          };

          const doStreamGroupsGetStreamGroupUpdate = (data: iptv.StreamGroupDto) => {
            // console.log('doStreamGroupsGetStreamGroupUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['StreamGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('StreamGroupsRefresh', doStreamGroupsGetStreamGroupUpdate);

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
            updateCachedData((draft: iptv.StreamGroupsGetStreamGroupEpgForGuideApiResponse) => {
              draft=data
              return draft;
            });
          };

          const doStreamGroupsGetStreamGroupEpgForGuideUpdate = (data: iptv.EpgGuide) => {
            // console.log('doStreamGroupsGetStreamGroupEpgForGuideUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['StreamGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('StreamGroupsRefresh', doStreamGroupsGetStreamGroupEpgForGuideUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    streamGroupsGetPagedStreamGroups: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StreamGroupDto[]) => {
            updateCachedData((draft: iptv.StreamGroupsGetPagedStreamGroupsApiResponse) => {
              data.forEach(item => {
                const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft.data[index] = item;
                }
              });

              return draft;
            });
          };

          const doStreamGroupsGetPagedStreamGroupsUpdate = (data: iptv.StreamGroupDto[]) => {
            // console.log('doStreamGroupsGetPagedStreamGroupsUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['StreamGroups']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('StreamGroupsRefresh', doStreamGroupsGetPagedStreamGroupsUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
