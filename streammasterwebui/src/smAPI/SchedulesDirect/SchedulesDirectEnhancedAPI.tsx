import { hubConnection } from '../../app/signalr';
import { isEmptyObject } from '../../common/common';
import { iptvApi } from '../../store/iptvApi';
import type * as iptv from '../../store/iptvApi';

export const enhancedApiSchedulesDirect = iptvApi.enhanceEndpoints({
  endpoints: {
    schedulesDirectGetCountries: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.Countries) => {
            updateCachedData((draft: iptv.SchedulesDirectGetCountriesApiResponse) => {
              draft=data
              return draft;
            });
          };

          const doSchedulesDirectGetCountriesUpdate = (data: iptv.Countries) => {
            // console.log('doSchedulesDirectGetCountriesUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('SchedulesDirectRefresh', doSchedulesDirectGetCountriesUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    schedulesDirectGetLineup: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.LineUpResult) => {
            updateCachedData((draft: iptv.SchedulesDirectGetLineupApiResponse) => {
              draft=data
              return draft;
            });
          };

          const doSchedulesDirectGetLineupUpdate = (data: iptv.LineUpResult) => {
            // console.log('doSchedulesDirectGetLineupUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('SchedulesDirectRefresh', doSchedulesDirectGetLineupUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    schedulesDirectGetLineupPreviews: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.LineUpPreview[]) => {
            updateCachedData((draft: iptv.SchedulesDirectGetLineupPreviewsApiResponse) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          const doSchedulesDirectGetLineupPreviewsUpdate = (data: iptv.LineUpPreview[]) => {
            // console.log('doSchedulesDirectGetLineupPreviewsUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('SchedulesDirectRefresh', doSchedulesDirectGetLineupPreviewsUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    schedulesDirectGetLineups: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.LineUpsResult) => {
            updateCachedData((draft: iptv.SchedulesDirectGetLineupsApiResponse) => {
              draft=data
              return draft;
            });
          };

          const doSchedulesDirectGetLineupsUpdate = (data: iptv.LineUpsResult) => {
            // console.log('doSchedulesDirectGetLineupsUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('SchedulesDirectRefresh', doSchedulesDirectGetLineupsUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    schedulesDirectGetStationPreviews: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StationPreview[]) => {
            updateCachedData((draft: iptv.SchedulesDirectGetStationPreviewsApiResponse) => {
              data.forEach(item => {
                const index = draft.findIndex(existingItem => existingItem.id === item.id);
                if (index !== -1) {
                  draft[index] = item;
                }
              });

              return draft;
            });
          };

          const doSchedulesDirectGetStationPreviewsUpdate = (data: iptv.StationPreview[]) => {
            // console.log('doSchedulesDirectGetStationPreviewsUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('SchedulesDirectRefresh', doSchedulesDirectGetStationPreviewsUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
    schedulesDirectGetStatus: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.SdStatus) => {
            updateCachedData((draft: iptv.SchedulesDirectGetStatusApiResponse) => {
              draft=data
              return draft;
            });
          };

          const doSchedulesDirectGetStatusUpdate = (data: iptv.SdStatus) => {
            // console.log('doSchedulesDirectGetStatusUpdate')
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          }

          hubConnection.on('SchedulesDirectRefresh', doSchedulesDirectGetStatusUpdate);

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
      }
    },
  }
});
