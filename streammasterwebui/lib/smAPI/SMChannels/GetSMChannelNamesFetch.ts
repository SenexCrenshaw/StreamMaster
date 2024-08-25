import { GetSMChannelNames } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetSMChannelNames = createAsyncThunk('cache/getGetSMChannelNames', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetSMChannelNames');
  const fetchDebug = localStorage.getItem('fetchDebug');
 const start = performance.now();
    const response = await GetSMChannelNames();
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


