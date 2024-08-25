import { GetChannelGroupsFromSMChannels } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetChannelGroupsFromSMChannels = createAsyncThunk('cache/getGetChannelGroupsFromSMChannels', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetChannelGroupsFromSMChannels');
  const fetchDebug = localStorage.getItem('fetchDebug');
 const start = performance.now();
    const response = await GetChannelGroupsFromSMChannels();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetM3UFiles completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


