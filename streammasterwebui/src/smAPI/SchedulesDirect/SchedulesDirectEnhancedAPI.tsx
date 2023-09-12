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
            updateCachedData((draft: iptv.Countries) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('SchedulesDirectRefresh', (data: iptv.Countries) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('SchedulesDirectRefresh');
      }
    },
    schedulesDirectGetHeadends: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.HeadendDto[]) => {
            updateCachedData((draft: iptv.HeadendDto[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('SchedulesDirectRefresh', (data: iptv.HeadendDto[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('SchedulesDirectRefresh');
      }
    },
    schedulesDirectGetLineup: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.LineUpResult) => {
            updateCachedData((draft: iptv.LineUpResult) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('SchedulesDirectRefresh', (data: iptv.LineUpResult) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('SchedulesDirectRefresh');
      }
    },
    schedulesDirectGetLineupPreviews: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.LineUpPreview[]) => {
            updateCachedData((draft: iptv.LineUpPreview[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('SchedulesDirectRefresh', (data: iptv.LineUpPreview[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('SchedulesDirectRefresh');
      }
    },
    schedulesDirectGetLineups: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.LineUpsResult) => {
            updateCachedData((draft: iptv.LineUpsResult) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('SchedulesDirectRefresh', (data: iptv.LineUpsResult) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('SchedulesDirectRefresh');
      }
    },
    schedulesDirectGetSchedules: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.Schedule[]) => {
            updateCachedData((draft: iptv.Schedule[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('SchedulesDirectRefresh', (data: iptv.Schedule[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('SchedulesDirectRefresh');
      }
    },
    schedulesDirectGetStationPreviews: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.StationPreview[]) => {
            updateCachedData((draft: iptv.StationPreview[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('SchedulesDirectRefresh', (data: iptv.StationPreview[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('SchedulesDirectRefresh');
      }
    },
    schedulesDirectGetStations: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.Station[]) => {
            updateCachedData((draft: iptv.Station[]) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('SchedulesDirectRefresh', (data: iptv.Station[]) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('SchedulesDirectRefresh');
      }
    },
    schedulesDirectGetStatus: {
      async onCacheEntryAdded(api, { dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.SdStatus) => {
            updateCachedData((draft: iptv.SdStatus) => {
              draft=data
              return draft;
            });
          };

          hubConnection.on('SchedulesDirectRefresh', (data: iptv.SdStatus) => {
            if (isEmptyObject(data)) {
              dispatch(iptvApi.util.invalidateTags(['SchedulesDirect']));
            } else {
              updateCachedDataWithResults(data);
            }
          });

        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }

        await cacheEntryRemoved;
        hubConnection.off('SchedulesDirectRefresh');
      }
    },
  }
});
