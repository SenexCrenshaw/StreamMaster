import { isDev } from '@/lib/settings';
import { singletonVideoStreamsListener } from '@/lib/signalr/singletonListeners';
import { isEmptyObject } from '@/lib/common/common';
import isPagedTableDto from '@/lib/common/isPagedTableDto';
import { iptvApi } from '@/lib/iptvApi';
import type * as iptv from '@/lib/iptvApi';

export const enhancedApiVideoStreams = iptvApi.enhanceEndpoints({
  endpoints: {
    videoStreamsGetChannelLogoDtos: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelLogoDto[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('empty', data);
              dispatch(iptvApi.util.invalidateTags(['VideoStreams']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'VideoStreams' }])) {
                if (endpointName !== 'videoStreamsGetChannelLogoDtos') continue;
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {

                      if (isPagedTableDto(data)) {
                      data.forEach(item => {
                        const index = draft.findIndex(existingItem => existingItem.id === item.id);
                        if (index !== -1) {
                          draft[index] = item;
                        }
                        });

                        return draft;
                        }

                      data.forEach(item => {
                        const index = draft.findIndex(existingItem => existingItem.id === item.id);
                        if (index !== -1) {
                          draft[index] = item;
                        }
                        });

                      return draft;
                     })
                   )
                 }


            });
          };

         singletonVideoStreamsListener.addListener(updateCachedDataWithResults);

        await cacheEntryRemoved;
        singletonVideoStreamsListener.removeListener(updateCachedDataWithResults);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
    videoStreamsGetVideoStream: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.VideoStreamDto) => {
            updateCachedData(() => {
              if (isDev) console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'VideoStreams' }])) {
                if (endpointName !== 'videoStreamsGetVideoStream') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    if (isDev) console.log('updateCachedData', data, draft);
                   })
                   );
                 }


            });
          };

         singletonVideoStreamsListener.addListener(updateCachedDataWithResults);

        await cacheEntryRemoved;
        singletonVideoStreamsListener.removeListener(updateCachedDataWithResults);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
    videoStreamsGetPagedVideoStreams: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.VideoStreamDto[]) => {
            if (!data || isEmptyObject(data)) {
              if (isDev) console.log('empty', data);
              dispatch(iptvApi.util.invalidateTags(['VideoStreams']));
              return;
            }

            updateCachedData(() => {
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'VideoStreams' }])) {
                if (endpointName !== 'videoStreamsGetPagedVideoStreams') continue;
                  dispatch(
                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {

                      if (isPagedTableDto(data)) {
                      data.forEach(item => {
                        const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                        if (index !== -1) {
                          draft.data[index] = item;
                        }
                        });

                        return draft;
                        }

                      data.forEach(item => {
                        const index = draft.data.findIndex(existingItem => existingItem.id === item.id);
                        if (index !== -1) {
                          draft.data[index] = item;
                        }
                        });

                      return draft;
                     })
                   )
                 }


            });
          };

         singletonVideoStreamsListener.addListener(updateCachedDataWithResults);

        await cacheEntryRemoved;
        singletonVideoStreamsListener.removeListener(updateCachedDataWithResults);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
  }
});
