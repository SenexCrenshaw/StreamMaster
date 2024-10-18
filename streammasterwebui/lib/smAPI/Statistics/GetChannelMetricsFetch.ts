import { GetChannelMetrics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetChannelMetrics = createAsyncThunk('cache/getGetChannelMetrics', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetChannelMetrics');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetChannelMetrics();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetChannelMetrics completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


