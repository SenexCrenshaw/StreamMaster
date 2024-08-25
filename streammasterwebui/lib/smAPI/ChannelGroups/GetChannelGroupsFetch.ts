import { GetChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetChannelGroups = createAsyncThunk('cache/getGetChannelGroups', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetChannelGroups');
  const fetchDebug = localStorage.getItem('fetchDebug');
 const start = performance.now();
    const response = await GetChannelGroups();
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


