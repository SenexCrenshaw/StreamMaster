import { GetChannelMetrics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetChannelMetrics = createAsyncThunk('cache/getGetChannelMetrics', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetChannelMetrics');
    const response = await GetChannelMetrics();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


