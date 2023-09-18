import { singletonChannelGroupsListener } from '../../app/createSingletonListener';
import { isEmptyObject } from '../../common/common';
import isPagedTableDto from '../../components/dataSelector/isPagedTableDto';
import type * as iptv from '../../store/iptvApi';
import { iptvApi } from '../../store/iptvApi';

export const enhancedApiChannelGroups = iptvApi.enhanceEndpoints({
  endpoints: {
    channelGroupsGetChannelGroup: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupDto) => {
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'ChannelGroups' }])) {
                if (endpointName !== 'channelGroupsGetChannelGroup') continue;
                dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                  console.log('updateCachedData', data, draft);
                })
                );
              }


            });
          };

          singletonChannelGroupsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonChannelGroupsListener.removeListener(updateCachedDataWithResults);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
    channelGroupsGetChannelGroupIdNames: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupIdName[]) => {
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'ChannelGroups' }])) {
                if (endpointName !== 'channelGroupsGetChannelGroupIdNames') continue;
                dispatch(
                  iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    if (isEmptyObject(data)) {
                      console.log('empty', data);
                      dispatch(iptvApi.util.invalidateTags(['ChannelGroups']));
                      return;
                    }

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

          singletonChannelGroupsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonChannelGroupsListener.removeListener(updateCachedDataWithResults);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
    channelGroupsGetChannelGroupsForStreamGroup: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupDto[]) => {
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'ChannelGroups' }])) {
                if (endpointName !== 'channelGroupsGetChannelGroupsForStreamGroup') continue;
                dispatch(
                  iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    if (isEmptyObject(data)) {
                      console.log('empty', data);
                      dispatch(iptvApi.util.invalidateTags(['ChannelGroups']));
                      return;
                    }

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

          singletonChannelGroupsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonChannelGroupsListener.removeListener(updateCachedDataWithResults);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
    channelGroupsGetPagedChannelGroups: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.ChannelGroupDto[]) => {
            updateCachedData(() => {
              console.log('updateCachedData', data);
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'ChannelGroups' }])) {
                if (endpointName !== 'channelGroupsGetPagedChannelGroups') continue;
                dispatch(
                  iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {
                    if (isEmptyObject(data)) {
                      console.log('empty', data);
                      dispatch(iptvApi.util.invalidateTags(['ChannelGroups']));
                      return;
                    }

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

          singletonChannelGroupsListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonChannelGroupsListener.removeListener(updateCachedDataWithResults);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

      }
    },
  }
});
