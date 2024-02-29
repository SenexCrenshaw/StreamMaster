import { isDev } from '@lib/settings';
import { singletonProfilesListener } from '@lib/signalr/singletonListeners';
import { isEmptyObject } from '@lib/common/common';
import isPagedTableDto from '@lib/common/isPagedTableDto';
import { iptvApi } from '@lib/iptvApi';
import type * as iptv from '@lib/iptvApi';

export const enhancedApiProfiles = iptvApi.enhanceEndpoints({
  endpoints: {
    profilesGetFFMPEGProfiles: {
      async onCacheEntryAdded(api, { dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }) {
        try {
          await cacheDataLoaded;

          const updateCachedDataWithResults = (data: iptv.FFMPEGProfileDtos) => {
            updateCachedData(() => {{
              if (isDev) console.log('updateCachedData', data);
              if (!data) {
                dispatch(iptvApi.util.invalidateTags(['Profiles']));
                return;
              }
              for (const { endpointName, originalArgs } of iptvApi.util.selectInvalidatedBy(getState(), [{ type: 'Profiles' }])) {
                if (endpointName !== 'profilesGetFFMPEGProfiles') continue;
                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {{
                    if (isDev) console.log('updateCachedData', data, draft);
                   }})
                   );
                 }}
            });
          };

          singletonProfilesListener.addListener(updateCachedDataWithResults);

          await cacheEntryRemoved;
          singletonProfilesListener.removeListener(updateCachedDataWithResults);
        } catch (error) {
          console.error('Error in onCacheEntryAdded:', error);
        }
      }
    // eslint-disable-next-line comma-dangle
    },
  }
});
